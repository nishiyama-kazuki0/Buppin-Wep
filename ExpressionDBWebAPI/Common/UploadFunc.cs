using Anotar.Serilog;
using ExpressionDBWebAPI.Controllers;
using ExpressionDBWebAPI.Data;
using SharedModels;
using System.Data;
using System.Text;

namespace ExpressionDBWebAPI.Common;

public class UploadFunc
{
    #region ファイル取込

    /// <summary>
    /// ファイル取込
    /// </summary>
    public static (int retCode, string retMsg) ファイル取込(string DEVICE_ID, string USER_ID, string ProgramName, string FilePath, string ClassName)
    {
        (int retCode, string retMsg) ret = (0, string.Empty);
        try
        {
            LogTo.Information($"DEVICE_ID:{DEVICE_ID},USER_ID:{USER_ID},ProgramName:{ProgramName},FilePath:{FilePath},ClassName:{ClassName}");

            // ファイルが存在するかつ、ファイル名が渡された名称から始まること
            if (!File.Exists(FilePath))
            {
                LogTo.Fatal("ファイルが存在しません");
                return (-1, "ファイルが存在しません");
            }
            else if (0 != Path.GetFileName(FilePath).IndexOf(ClassName))
            {
                LogTo.Fatal("ファイル種類が異なります");
                return (-1, "ファイル種類が異なります");
            }

            // 拡張子取得(ピリオドなし)
            string extension = Path.GetExtension(FilePath);
            extension = extension.Replace(".", "");

            // セパレートファイル種類決定
            SeparatedFileInfo? info = null;
            if (!string.IsNullOrEmpty(extension))
            {
                info = SharedConst.SeparatedFileInfos.FirstOrDefault(_ => _.Extension.ToLower() == extension.ToLower());
            }
            info ??= SharedConst.SeparatedFileInfos[0];

            using StreamReader sr = new(FilePath, info.FileEncoding);
            List<List<ArgumentValue>> argumentValues = [];
            List<string> titleData = [];
            int row = 0;
            while (sr.Peek() != -1)
            {
                string str = sr.ReadLine()!;
                if (row == 0)
                {
                    // 一行目はタイトル先頭の#を取り除く
                    str = str[1..];
                    titleData = new List<string>(str.Split(info.Delimiter));
                }
                else
                {
                    List<string> rowData = new(str.Split(info.Delimiter));
                    List<ArgumentValue> argumentValue = [];
                    for (int i = 0; titleData.Count > i && rowData.Count > i; i++)
                    {
                        argumentValue.Add(ArgumentValue.CreateArgumentValue(titleData[i], rowData[i], ""));
                    }
                    argumentValues.Add(argumentValue);
                }
                row++;
            }
            sr.Close();

            RequestValue requestValue = RequestValue.CreateRequestProgram(ProgramName);
            // 共通情報をセットする
            _ = requestValue.SetArgumentValue(SharedConst.KEY_CLASS_NAME, ClassName, string.Empty);
            _ = requestValue.SetArgumentValue(SharedConst.KEY_USER_ID, USER_ID, string.Empty);
            _ = requestValue.SetArgumentValue(SharedConst.KEY_DEVICE_ID, DEVICE_ID, string.Empty);
            // テーブル情報をセットする
            _ = requestValue.SetArgumentDataset(string.Empty, argumentValues);
            // =====>
            //  取込CSVの整合性チェック
            //   ・チェックされる側(CSVの内容)は、argumentValuesに格納されています。
            //   ・DEFINE_PROCESS_FUNCTIONSのARGUMENT_TYPE_NAME99が"structured"の場合と同じく
            //     ユーザー定義テーブル(ut_XXXXX)のカラム情報を取得し、項目名、型、桁数のチェックが可能
            //   ・ユーザー定義テーブルのカラム情報は、ExecutionController.csの489行目あたりからの手続き
            //     と同じ方法で取得可能と思われます。
            // <=====
            //先頭1つを取ってProcessProgramの設定情報を処理する。

            //プログラム名からプログラムの設定情報を取得する
            string[] paramName = ["ProgramName"];
            using IDbConnection con = DataSource.GetNewConnection();
            IEnumerable<ProcessFunctionModel> functionList = DataSource.GetEntityCollection<ProcessFunctionModel>(
                con
                , DataSource.CreateTableFunctionQuery("GetProcessFunctionsDefine", paramName, [("EXEC_ORDER_RANK", false)])
                , DataSource.CreateParamTable(new Dictionary<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)>
                    {
                        { "ProgramName", (ProgramName , 128 , typeof(string),null) }
                    }
                )
            );
            //functionListのなかから、ARGUMENT_TYPE_NAME1～30にstjが含まれているオブジェクトを抽出する
            //取り込み処理のPROCESS_PROGRAM内でユーザーテーブル型は1ファンクションしかありえないなので、Firstで取得する
            ProcessFunctionModel f = functionList.First(_ => _.IsStructuredContain == true);
            //LogTo.Information($"p.PROGRAM_NAME:[{p.PROGRAM_NAME}]");
            (int retCode, string retMsg) cr = CheckImportFileDataFormat(f, requestValue, con, info.FileEncoding);
            if (cr.retCode < 0)
            {
                return cr;
            }

            //ファイル内容の取り込み登録を行う。Executionに問い合わせてストアドを実行させる
            ExecutionController controller = new();
            Microsoft.AspNetCore.Mvc.ActionResult<IEnumerable<ExecResult>> result = controller.Post(requestValue);

            // エラーがある場合、エラーコードとメッセージを返す
            List<ExecResult> lstResult = result.Value.ToList();
            bool isError = false;
            foreach (ExecResult item in lstResult)
            {
                if (item.RetCode < 0)
                {
                    isError = true;
                    ret = (item.RetCode, item.Message);
                    break;
                }
            }
            // エラーがない場合、最後のメッセージを返す
            if (!isError && lstResult.Count > 0)
            {
                ret = (lstResult[^1].RetCode, lstResult[^1].Message);
            }
        }
        catch (Exception ex)
        {
            LogTo.Fatal(ex.Message);
            return ret = (-1, ex.Message);
        }
        finally
        {
            // 取込ファイルを削除する
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
        return ret;
    }

    #endregion

    #region チェック処理

    /// <summary>
    /// 取り込みファイルのフォーマットをユーザーデータテーブル型のフォーマットと突き合わせチェックを行う。
    /// </summary>
    /// <param name="f">ファンクションモデル</param>
    /// <param name="r">リクエストデータ</param>
    /// <param name="con">接続(何度かSELECTするため受け継ぐ)</param>
    /// <param name="fenc">ファイルのエンコード。ファイル内容のバイト桁数をチェックする必要があるため</param>
    /// <returns>retCodeが0以上:問題なし,retCodeが負数:問題あり</returns>
    private static (int retCode, string retMsg) CheckImportFileDataFormat(ProcessFunctionModel f, RequestValue r, IDbConnection con, Encoding fenc)
    {

        //ユーザー定義テーブル(ut_XXXXX)のカラム情報を取得
        (int retCode, string retMsg) = (int.MinValue, string.Empty);
        int successCode = 0;
        int errorCode = -1;
        //DataSource.ExecuteStoredFunctionを使用して、ProcessFunctionModel fのFUNCTION_NAMEの関数を実行する。
        //ストアド名から引数情報を取得する
        string[] paramName = ["Function"];
        IEnumerable<ParameterTypeModel> ParameterTypeList = DataSource.GetEntityCollection<ParameterTypeModel>(
                con
            , DataSource.CreateTableFunctionQuery("GetObjectParametersDefine", paramName, [("parameter_id", false)])
           , DataSource.CreateParamTable(new Dictionary<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)>
                    {
                        { "Function", (f.FUNCTION_NAME , 128 , typeof(string),null) }
                    }
                )
            );

        //ユーザーテーブル型のみ抽出する //1ストアドに2つ以上の要素はあり得ない思想。
        var ut = ParameterTypeList.First(_ =>
                                            //_.name.Replace(DataSource.ParamPrefixStr, string.Empty)
                                            //    == argName.Replace(DataSource.ParamPrefixStr, string.Empty)//定義テーブルにはパラメータ頭文字は付けない想定だが伝のためリプレイスしておく
                                            //    &&
                                            _.is_user_defined == true //ユーザーテーブル型のみ抽出する
                                        ); //取り込みのチェックでは、必ずユーザーテーブル型が含まれている想定ので、フラグのみで抽出して問題ないとする。存在しない場合はエラーなので、firstで取得してよいとする

        //ユーザーテーブル型からカラム情報を取得する
        paramName = new string[] { "UserTableTypeName" };
        var userTableTypeColumnList = DataSource.GetEntityCollection<UserTableTypeColumnModel>(
                con
            , DataSource.CreateTableFunctionQuery("GetObjectTableTypeColumnsDefine", paramName, [("ColumnId", false)])
            , DataSource.CreateParamTable(new Dictionary<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)>
                    {
                        { "UserTableTypeName", ( ut.type , 128 , typeof(string),null) }
                    }
                )
            );
        int lc = 1;//表示用行数カウンタ //ヘッダは含まないので1からとして定義する。

        //★★★★★★
        LogTo.Error("CheckImportFileDataFormat【要修正】1ストアドに2つ以上の要素はあり得ない思想。");
        //★★★★★★

        foreach (var rrr in r.ArgumentDataSet.Values)
        {
            foreach (var reqRow in rrr)
            {
                List<ArgumentValue> argValues = reqRow.Value;
                //項目数は必ずしもユーザー定義テーブルと一致するわけではないので、ここではチェックしない。
                //DBに必要な項目しかユーザー定義テーブルに宣言しないため。予定取り込みなど。
                //if (argValues.Count != userTableTypeColumnList.Count())
                //{
                //    return (errorCode, $"{lc}行目の項目数が不正です。");
                //}
                foreach (UserTableTypeColumnModel utColumn in userTableTypeColumnList)
                {
                    ArgumentValue? argval = argValues.FirstOrDefault(_ => _.ArgumentName == utColumn.ColumnName);
                    if (argval is null)
                    {
                        return (errorCode, $"{lc}行目の{utColumn.ColumnName}の項目がファイル内に存在しません。");
                    }
                    if (!CheckDBTypeForValue(utColumn.DataType, argval.Value.ToString()))//ファイル内容は必ず文字列のため、tostringで比較してよいとする
                    {
                        return (errorCode, $"{lc}行目の{utColumn.ColumnName}のデータ型が不正です。");
                    }
                    if (utColumn.DataType == "bit")//bit型は桁数の判定をスルーさせる
                    {
                        //bit型はCheckDBTypeForValueでチェックしているため、ここではループを継続させる
                        continue;
                    }
                    //桁チェック
                    int cnt = fenc.GetByteCount(argval.Value.ToString() ?? string.Empty);
                    bool isUseMaxLength = DataSource.IsSizeSpecNeeded(utColumn.DataType);
                    if (isUseMaxLength && cnt > utColumn.MaxLength)// 文字列はバイト数で判断する 
                    {
                        return (errorCode, $"{lc}行目の{utColumn.ColumnName}の桁数が不正です。最大桁は{utColumn.MaxLength}です。");
                    }
                    else if (!isUseMaxLength && cnt > utColumn.Precision) //数値の場合はPrecisionで判断する。MaxLengthはバイト数のため。
                    {
                        return (errorCode, $"{lc}行目の{utColumn.ColumnName}の桁数が不正です。最大桁は{utColumn.Precision}です。");
                    }
                    else
                    {

                    }

                }
                lc++;
            }
        }
        return (successCode, retMsg);
    }

    /// <summary>
    /// 型チェック(数値型、日付型の変換)
    /// </summary>
    /// <param name="type">データ型</param>
    /// <param name="value">値</param>
    /// <returns>型チェック結果true : ok , false NG</returns>
    private static bool CheckDBTypeForValue(string type, string? value)
    {
        //TODO Oracleの場合はDBの型名は問題ないか確認する必要がある
        bool isValid = type switch
        {
            "int" or "smallint" => int.TryParse(value, out _),
            "bigint" => long.TryParse(value, out _),
            "tinyint" => byte.TryParse(value, out _),
            "decimal" or "money" or "smallmoney" or "numeric" => decimal.TryParse(value, out _),
            "float" => double.TryParse(value, out _),
            "real" => float.TryParse(value, out _),
            "date" or "datetime" or "datetime2" or "smalldatetime" => DateTime.TryParse(value, out _),
            "bit" => bool.TryParse(value, out _),
            "varchar" or "char" or "nvarchar" or "nchar" => true,//文字列型は強制でOKとする
            _ => false,
        };
        return isValid;
    }
    #endregion
}
