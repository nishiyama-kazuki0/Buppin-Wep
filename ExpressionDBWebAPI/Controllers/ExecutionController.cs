using Anotar.Serilog;
using DynamicExpresso;
using ExpressionDBWebAPI.Common;
using ExpressionDBWebAPI.Data;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Parameter = DynamicExpresso.Parameter;

namespace ExpressionDBWebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ExecutionController : ControllerBase
{
    //TODO コンストラクタはMoqテスト用 xUnitテストで必要だったため追加。悪さをする場合はコメント化する
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ExecutionController()
    {
    }
    ///// <summary>
    ///// コンストラクタ 
    ///// </summary>
    ///// <param name="object"></param>
    //public ExecutionController(IHttpClientFactory @object)
    //{
    //    Object = @object;
    //}
    ///// <summary>
    ///// Moqテスト用にDIされるIHttpClientFactory
    ///// </summary>
    //public IHttpClientFactory? Object { get; }

    private const int successRetCode = 0;
    private const int alarmWebAPIRetCode = -99;
    private const string pName_OperateLogRegist = "操作ログ登録";//TODO 個別で定義したくないが暫定
    private const string pName_NotifyLogRegist = "通知ログ登録";//TODO 個別で定義したくないが暫定
    //ストアドのメッセージを分割するための文字列
    private const string splitMessageStr = "||";//TODO 個別で定義したくないが暫定
    //ストアドから返ってくる区分は$$で囲まれている前提
    private const string splitNotifyCatePattern = "\\$\\$[^\\$]+\\$\\$";//TODO 個別で定義したくないが暫定
    private const string replaceNotifyCateStr = "$$";//TODO 個別で定義したくないが暫定

    /// <summary>
    /// リクエスト動的実行コントローラー
    /// </summary>
    /// <param name="reqValue"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult<IEnumerable<ExecResult>> Post([FromBody] RequestValue reqValue, CancellationToken cancellationToken = default, bool isRePost = false)
    {
        LogTo.Information($"reqValue.RequestProgramName:[{reqValue.RequestProgramName}]");
        List<ExecResult> ret = [];

        //RepoDBを使用してDBから値を取得します。
        try
        {

            using IDbConnection con = DataSource.GetNewConnection();
            //デバイス名を取得する
            if (!isRePost) //isRePostがtrueの場合はhttpRequestが閉じられている可能性があるため省く
            {
                string remotePcName = CommonFunc.GetDeviceID(Request, con);
                _ = reqValue.SetArgumentValue("DEVICE_ID", remotePcName, "");
            }
            //プログラム名からプログラムの設定情報を取得する
            string[] paramName = ["ProgramName"];
            IEnumerable<ProcessFunctionModel> functionList = DataSource.GetEntityCollection<ProcessFunctionModel>(
                con
                , DataSource.CreateTableFunctionQuery("GetProcessFunctionsDefine", paramName, [("EXEC_ORDER_RANK", false)])
                , DataSource.CreateParamTable(new Dictionary<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)>
                    {
                        { "ProgramName", (reqValue.RequestProgramName , 128 , typeof(string),null) }
                    }
                )
            );
            //先頭1つを取ってProcessProgramの設定情報を処理する。
            //トランザクションを張る、semaphoreSlimの縛りなど。
            ProcessFunctionModel p = functionList.First();
            LogTo.Information($"p.PROGRAM_NAME:[{p.PROGRAM_NAME}]");

            //プログラム全体トランザクションが必要な場合はトランザクションを張る
            using IDbTransaction? tran = p.PROGRAM_IS_TRANSACTION ? DataSource.BeginTransactionAndOpenEnsure(con) : null;

            //SemaphoreSlim ss = new(p.PROGRAM_SEMAPHOE_MAX_COUNT, p.PROGRAM_SEMAPHOE_MAX_COUNT);

            //ログに出力するための引数文字列を生成する。
            var msgArgumentValues = "";
            try {
                msgArgumentValues = CreateArgumentValueString(reqValue);
            } catch {
                //ログ引数生成で例外発生しても無視する
            }

            try
            {

                //Semaphoreでスレッドの実行を制御する。
                //TODO 異なるプログラム名の場合は影響しないようにする必要あり。
                //ss.Wait();


                foreach (ProcessFunctionModel f in functionList)
                {
                    LogTo.Information($"f.FUNCTION_NAME:[{f.FUNCTION_NAME}]");

                    if (!reqValue.IsCancelTokenIgnore)
                    {
                        //クライアント側からキャンセル要求があった場合は処理を中断する。
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    //操作ログを登録する。完了は待たないため、非同期で実行
                    //registOperateLogTask(reqValue, successRetCode, "実行開始", f);

                    //各ファンクションタイプを見て実行を行う//TODO Interfaceを使用してFactoryパターンで分ける、など機能が増えそうな場合は検討する
                    (int retCode, string retMsg) = f.FUNCTION_TYPE switch
                    {
                        (int)ProcessFunctionModel.enumFunctionType.DotNetFunction => ExecuteDotNETFunction(f, reqValue),
                        (int)ProcessFunctionModel.enumFunctionType.StoredFunction => ExecuteStoredFunction(f, reqValue, tran),
                        (int)ProcessFunctionModel.enumFunctionType.BatchFileExec => ExecuteOuterFileProcess(f, reqValue),
                        _ => (int.MinValue, string.Empty)
                    };
                    //実行結果のログ追加
                    LogTo.Information($"execResult " +
                                $"ProgramName:[{f.PROGRAM_NAME}]" +
                                $",FunctionName:[{f.FUNCTION_NAME}]" +
                                $",ExecOrderRank:[{f.EXEC_ORDER_RANK}]" +
                                $",RetCode:[{retCode}]" +
                                $",Message[{retMsg}]");
                    //処理結果を追加する
                    if (retCode >= successRetCode && retMsg.Contains(splitMessageStr))
                    {
                        //スプリット文字が含まれている場合は分割する
                        string[] splitMsg = retMsg.Split(splitMessageStr);
                        foreach (string msg in splitMsg)
                        {
                            int splitRetCode = successRetCode;
                            string splitMsgRet = msg;
                            //分割されたメッセージから区分を取得する
                            Match? match = Regex.Matches(msg, splitNotifyCatePattern).FirstOrDefault();
                            if (match is not null)
                            {
                                splitMsgRet = msg.Replace(match.Value, string.Empty);
                                string m = match.Value.Replace(replaceNotifyCateStr, string.Empty);
                                splitRetCode = Convert.ToInt32(m);
                            }
                            if (!string.IsNullOrWhiteSpace(msg) || !string.IsNullOrWhiteSpace(splitMsgRet))
                            {
                                ret.Add(
                                new ExecResult(
                                    f.PROGRAM_NAME
                                    , f.FUNCTION_NAME
                                    , f.EXEC_ORDER_RANK
                                    , splitRetCode
                                    , splitMsgRet
                                    )
                                );
                                registNotifyLogTask(reqValue, splitRetCode, splitMsgRet, f);
                                registOperateLogTask(reqValue, splitRetCode, $"{splitMsgRet}\t{msgArgumentValues}", f);
                            }
                        }
                        //この時点で、万が一、追加した結果の件数が0件なら何かresultを追加しないといけない。むしろ例外なのでスローしても良い
                        if (!ret.Any())
                        {
                            ret.Add(
                            new ExecResult(
                                f.PROGRAM_NAME
                                , f.FUNCTION_NAME
                                , f.EXEC_ORDER_RANK
                                , retCode
                                , retMsg.Replace(splitMessageStr, string.Empty).Replace(replaceNotifyCateStr, string.Empty)//念のため区切り文字などは除去する
                                )
                            );
                            registNotifyLogTask(reqValue, retCode, retMsg.Replace(splitMessageStr, string.Empty).Replace(replaceNotifyCateStr, string.Empty), f);
                            registOperateLogTask(reqValue, retCode, $"{retMsg.Replace(splitMessageStr, string.Empty).Replace(replaceNotifyCateStr, string.Empty)}\t{msgArgumentValues}", f);
                        }
                    }
                    else
                    {
                        ret.Add(
                        new ExecResult(
                            f.PROGRAM_NAME
                            , f.FUNCTION_NAME
                            , f.EXEC_ORDER_RANK
                            , retCode
                            , retMsg.Replace(splitMessageStr, string.Empty).Replace(replaceNotifyCateStr, string.Empty)//念のため区切り文字などは除去する
                            )
                        );
                        registNotifyLogTask(reqValue, retCode, retMsg.Replace(splitMessageStr, string.Empty).Replace(replaceNotifyCateStr, string.Empty), f);
                        registOperateLogTask(reqValue, retCode, $"{retMsg.Replace(splitMessageStr, string.Empty).Replace(replaceNotifyCateStr, string.Empty)}\t{msgArgumentValues}", f);
                    }
                    //クライアント側からキャンセル要求があった場合は処理を中断する。//処理終了時もキャンセルがかかっていないか確認する。できていない場合はロールバックとなる
                    if (!reqValue.IsCancelTokenIgnore)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    //処理結果が0以外の場合は処理を中断する。
                    if (retCode < successRetCode)
                    {
                        break;
                    }
                }
                //トランザクションのコミット、ロールバックを行う。トランザクションがない場合は何もしない。
                //retCodeが0以外のものが1件でも存在する場合、または0が1件も存在しない場合はロールバックする。//TODO 後者の条件は問題ないか検討必要。
                bool isCommit = !ret.Any(_ => _.RetCode < successRetCode);
                if (tran is not null && isCommit)
                {
                    //トランザクションをコミットする
                    DataSource.CommitTransactionAndCloseEnsure(tran);
                }
                else if (tran is not null && !isCommit)
                {
                    //トランザクションをロールバックする
                    DataSource.RollbackTransactionAndCloseEnsure(tran);
                }
            }
            catch (Exception ex)
            {
                LogTo.Fatal(ex.Message);
                if (tran is not null)
                {
                    //トランザクションをロールバックする
                    DataSource.RollbackTransactionAndCloseEnsure(tran);
                }
                //最初の一つが取得できるなら追加しておく
                ProcessFunctionModel? ep = functionList.FirstOrDefault();
                if (ep != null)
                {
                    ret.Add(
                        new ExecResult(
                            ep.PROGRAM_NAME
                            , string.Empty
                            , 0
                            , alarmWebAPIRetCode
                            , ex.Message
                            )
                        );
                    registNotifyLogTask(reqValue, alarmWebAPIRetCode, ex.Message, ep);
                    registOperateLogTask(reqValue, alarmWebAPIRetCode, $"{ex.Message}\t{msgArgumentValues}", ep);
                }
            }
            finally
            {
                //_ = ss.Release(p.PROGRAM_SEMAPHOE_MAX_COUNT);

            }
        }
        catch (Exception ex)
        {
            LogTo.Fatal(ex.Message);
        }

        //return Ok(null);
        //TODO これでよいか検討。ステータスをもどす必要はないか？
        return ret;


        string CreateArgumentValueString(RequestValue reqValue)
        {
            var msgArgumentValues = "";
            foreach (var n in reqValue.ArgumentValues)
            {
                msgArgumentValues += $",{n.Key}={n.Value.Value}";
            }
            if (msgArgumentValues.Length > 0) msgArgumentValues = msgArgumentValues.Substring(1);
            return msgArgumentValues;
        }

        //インナーメソッド :通知ログの登録
        void registNotifyLogTask(RequestValue req, int rc, string msg, ProcessFunctionModel pfm)
        {
            //通知ログを登録する。完了は待たないため、非同期で実行
            _ = Task.Run(() =>
            {
                if (pfm.LOG_TYPE is ((int)ProcessFunctionModel.enumDBLogType.NoneLog)
                        or ((int)ProcessFunctionModel.enumDBLogType.NoneNotifyLog)
                        || req.RequestProgramName == pName_NotifyLogRegist　//万が一ログタイプの設定漏れがあった場合に無限ループになるので保険で条件を追加しておく。
                        )
                {
                    //通知ログ登録のプログラムだと無限ループになるため戻る
                    return;
                }
                RequestValue c = req.Clone();
                c.RequestProgramName = pName_NotifyLogRegist;
                _ = c.SetArgumentValue("RESULT_CODE", rc, "")
                .SetArgumentValue("MESSAGE", msg, "")
                .SetArgumentValue("PROGRAM_NAME", pfm.PROGRAM_NAME, "")
                .SetArgumentValue("FUNCTION_NAME", pfm.FUNCTION_NAME, "")
                ;
                _ = Post(c, isRePost: true);
            });
        }
        //インナーメソッド 操作ログの登録
        void registOperateLogTask(RequestValue req, int rc, string msg, ProcessFunctionModel pfm)
        {
            //操作ログを登録する。完了は待たないため、非同期で実行
            _ = Task.Run(() =>
            {
                if (pfm.LOG_TYPE is ((int)ProcessFunctionModel.enumDBLogType.NoneLog)
                        or ((int)ProcessFunctionModel.enumDBLogType.NoneOperateLog)
                        || req.RequestProgramName == pName_OperateLogRegist　//万が一ログタイプの設定漏れがあった場合に無限ループになるので保険で条件を追加しておく。
                    )
                {
                    //操作ログ登録のプログラムだと無限ループになるため戻る
                    return;
                }
                RequestValue c = req.Clone();
                c.RequestProgramName = pName_OperateLogRegist;
                _ = c.SetArgumentValue("RESULT_CODE", rc, "")
                .SetArgumentValue("MESSAGE", msg, "")
                .SetArgumentValue("PROGRAM_NAME", pfm.PROGRAM_NAME, "")
                .SetArgumentValue("FUNCTION_NAME", pfm.FUNCTION_NAME, "")
                ;
                _ = Post(c, isRePost: true);
            });
        }
    }

    /// <summary>
    /// ファンクション名から.NETメソッドを実行する
    /// </summary>
    /// <param name="f"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    private static (int retCode, string retMsg) ExecuteDotNETFunction(ProcessFunctionModel f, RequestValue r)
    {
        _ = (int.MinValue, string.Empty);
        (int retCode, string retMsg) ret;
        // ExpressionCompilerを使用して、ProcessFunctionModel fのFUNCTION_NAMEの関数を実行する。
        // 自作クラスのメソッドを実行させる
        try
        {
            if (string.IsNullOrWhiteSpace(f.FUNCTION_NAME)
                || string.IsNullOrWhiteSpace(f.ASSEMBLY_NAME)
                || string.IsNullOrWhiteSpace(f.CLASS_NAME))
            {
                LogTo.Warning($"string.IsNullOrWhiteSpace(f.FUNCTION_NAME)||string.IsNullOrWhiteSpace(f.ASSEMBLY_NAME)||string.IsNullOrWhiteSpace(f.CLASS_NAME) is true");
                return ret = (alarmWebAPIRetCode, "string.IsNullOrWhiteSpace(f.FUNCTION_NAME)||string.IsNullOrWhiteSpace(f.ASSEMBLY_NAME)||string.IsNullOrWhiteSpace(f.CLASS_NAME) is true");
            }

            string className = $"{f.ASSEMBLY_NAME}.{f.CLASS_NAME}";
            Type? type = Type.GetType(className);
            if (type is null)
            {
                LogTo.Warning($"type is null");
                return ret = (alarmWebAPIRetCode, $"type is null className:[{className}]");
            }
            Assembly ass = type.Assembly;
            if (ass is null)
            {
                LogTo.Warning($"ass is null");
                return ret = (alarmWebAPIRetCode, $"ass is null className:[{className}]");
            }

            //以下動的コンパイルの処理。

            // ログ内容
            LogTo.Information($"f.FUNCTION_NAME:{f.FUNCTION_NAME}");

            //DynamicExpressoを使用して、f.FUNCTION_NAMEのメソッドを実行する
            Interpreter interpreter = new();

            _ = interpreter.Reference(type);
            StringBuilder sb = new();
            //ファンクションを実行する文字列を組み上げる
            _ = sb.Append($"{f.CLASS_NAME}.{f.FUNCTION_NAME}(");
            List<Parameter> param = [];
            for (int i = 0; i < f.ARGUMENT_COUNT; i++)
            {
                PropertyInfo? propertyInfoArgName = f.GetType().GetProperty($"ARGUMENT_NAME{i + 1}");
                if (propertyInfoArgName is null)
                {
                    LogTo.Information($"propertyInfoArgName is null");
                    ret = (alarmWebAPIRetCode, $"propertyInfoArgName is null :[ARGUMENT_NAME{i + 1}]");
                    continue;
                }
                //PROCESS_FUNCTIONSの定義されている引数名を取得
                string argName = $"{propertyInfoArgName.GetValue(f)}";
                //引数名をファンクション名に追記する
                _ = sb.Append($"{(i == 0 ? string.Empty : ",")}{argName}");//初回の引数はカンマなし

                //定義の引数名をもとにリクエストの引数値を取得する
                _ = r.ArgumentValues.TryGetValue(argName, out ArgumentValue? argVal);
                if (argVal is null)
                {
                    LogTo.Warning($"argVal is null");
                    //TODO エラー処理
                    continue;
                }
                //データタイプはPROCESS_FUNCTIONの定義を使用する。(クライアント側からはセットできない可能性が高いので)
                PropertyInfo? propertyInfoArgDataType = f.GetType().GetProperty($"ARGUMENT_TYPE_NAME{i + 1}");
                if (propertyInfoArgDataType is null)
                {
                    LogTo.Warning($"ropertyInfoArgDataType is null");
                    //TODO エラー処理 , またはobject型として決めてしまうか
                    continue;
                }
                string dataTypeString = $"{propertyInfoArgDataType.GetValue(f)}";
                Type? paramType;
                object? value = null;
                if (dataTypeString.Contains("System.Collections.Generic")) //TODO System.Collectionsに対応。他パターンが必要な気もする
                {
                    //ジェネリック型の場合は、文字列から型を取得する
                    paramType = GetTypeGenericFromString(dataTypeString);
                    paramType ??= typeof(object);//nullの場合はobject型として扱う
                    value = JsonSerializer.Deserialize(argVal.Value.ToString() ?? string.Empty, paramType);
                }
                else
                {
                    //メソッドに渡す引数を追加
                    paramType = Type.GetType(dataTypeString);
                    paramType ??= typeof(object);//nullの場合はobject型として扱う
                    value = Convert.ChangeType(argVal.Value.ToString(), paramType);
                }

                param.Add(new Parameter(argName, paramType, value));
            }
            _ = sb.Append($")");//閉じ括弧
            string functionString = sb.ToString();

            LogTo.Information($"functionString:{functionString}");
            //文字列からメソッドを実行 Eval : ParseとInvokeを行う。paramがカウント0の場合も問題なく実行される。void実行の場合、resultはnullが返る。
            object result = interpreter.Eval(functionString, param.ToArray());

            LogTo.Information($"result:{result}");
            if (f.IS_FUNCTION_RETURN)
            {
                //TODO 戻り値の取得処理整理必要。object型から変換しようとしたが、不要か？。

                PropertyInfo? propertyInfoReturnDataType = f.GetType().GetProperty($"FUNCTION_RETRUN_DATA_TYPE");
                if (propertyInfoReturnDataType is null)
                {
                    LogTo.Warning($"propertyInfoReturnDataType is null");
                    return ret = (alarmWebAPIRetCode, $"propertyInfoReturnDataType is null");
                }
                string returnDataTypeString = $"{propertyInfoReturnDataType.GetValue(f)}";

                if (string.IsNullOrWhiteSpace(returnDataTypeString))
                {
                    //DBに戻りの型が指定されてないときは(int,string)とする。
                    ret = ((int retCode, string retMsg))result;
                }
                else
                {
                    //TODO 戻りの型指定がある場合は何か対応する必要あり
                    ret = (successRetCode, string.Empty);
                }

                LogTo.Information($"ret.retCode:{ret.retCode},ret.retMsg:{ret.retMsg}");
            }
            else
            {
                //戻りなしは明示的にretCodeを0にしておく
                ret = (successRetCode, string.Empty);
            }
        }
        catch (Exception ex)
        {
            LogTo.Fatal(ex.Message);
            ret = (alarmWebAPIRetCode, ex.Message);
        }
        LogTo.Information($"f.FUNCTION_NAME:{f.FUNCTION_NAME},ret.retCode:{ret.retCode},ret.retMsg:{ret.retMsg}");
        return ret;
    }

    /// <summary>
    /// データタイプ文字列の先頭にSystem.Collections.Genericがあった場合に
    /// ジェネリック型のTypeを取得する。
    /// 文字列例:System.Collections.Generic.List`1[SharedModels.ChatMessageModel, SharedModels]
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static Type GetTypeGenericFromString(string typeName)
    {
        // 'System.Collections.Generic.List`1' 部分を取り出す
        int openBracketIndex = typeName.IndexOf('[');
        string genericTypeName = typeName[..openBracketIndex];

        // 'SharedModels.MyClass, SharedModels' 部分を取り出す
        //string genericArgumentName = typeName.Substring(openBracketIndex + 1, typeName.Length - openBracketIndex - 2);
        // コンマで分割してアセンブリ修飾型名を取得
        //string[] argumentParts = genericArgumentName.Split(',');
        string pattern = @"\[(.*?)\]";//[]で囲まれた部分を取得
        Match match = Regex.Match(typeName, pattern);
        string genericArgumentName = match.Groups[1].Value;

        // 型を取得
        Type? genericArgumentType = Type.GetType(genericArgumentName);

        if (genericArgumentType == null)
        {
            throw new Exception($"Failed to load generic argument type.[{genericArgumentName}]");
        }

        // リストのジェネリック型定義を取得
        Type? genericType = Type.GetType(genericTypeName);

        if (genericType == null)
        {
            throw new Exception($"Failed to load generic type.[{genericTypeName}]");
        }

        // ジェネリック型を作成
        Type listType = genericType.MakeGenericType(genericArgumentType);

        return listType;
    }

    /// <summary>
    /// ファンクション名からストアドを実行する
    /// </summary>
    /// <param name="f"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    private static (int retCode, string retMsg) ExecuteStoredFunction(ProcessFunctionModel f, RequestValue r, IDbTransaction? tran)
    {
        (int retCode, string retMsg) ret = (int.MinValue, string.Empty);
        //DataSource.ExecuteStoredFunctionを使用して、ProcessFunctionModel fのFUNCTION_NAMEの関数を実行する。

        //using SqlTransaction? unitTran = tran is null && f.IS_FUNCTION_TRANSACTION
        //    ? DataSource.BeginTransactionAndOpenEnsure(DataSource.GetNewConnection()) : null;

        try
        {
            //ストアド名から引数情報を取得する
            string[] paramName = ["FunctionName"];
            IEnumerable<ParameterTypeModel> ParameterTypeList = tran is null
                ? DataSource.GetEntityCollection<ParameterTypeModel>(
                DataSource.CreateTableFunctionQuery("GetObjectParametersDefine", paramName, [("parameter_id", false)])
                , DataSource.CreateParamTable(new Dictionary<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)>
                        {
                        { "FunctionName", (f.FUNCTION_NAME , 128 , typeof(string),null) }
                        }
                    )
                )
                : DataSource.GetEntityCollection<ParameterTypeModel>(
                    tran.Connection
                 , DataSource.CreateTableFunctionQuery("GetObjectParametersDefine", paramName, [("parameter_id", false)])
               , DataSource.CreateParamTable(new Dictionary<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)>
                        {
                        { "FunctionName", (f.FUNCTION_NAME , 128 , typeof(string),null) }
                        }
                    )
                , tran
                );

            Dictionary<string, object?> param = [];
            for (int i = 0; i < f.ARGUMENT_COUNT; i++)
            {
                PropertyInfo? pArgNameInfo = f.GetType().GetProperty($"ARGUMENT_NAME{i + 1}");
                PropertyInfo? pDataTypeInfo = f.GetType().GetProperty($"ARGUMENT_TYPE_NAME{i + 1}");
                if (pArgNameInfo is null
                    || pDataTypeInfo is null)
                {
                    LogTo.Warning($"pArgNameInfo is null || pDataTypeInfo is null is true");
                    //TODO エラー処理
                    continue;
                }
                //SqlDbType.Structuredの場合はArgumentDataSetのDatatable型の引数を作成する必要あり。
                string argDataType = $"{pDataTypeInfo.GetValue(f)}";
                string argName = $"{pArgNameInfo.GetValue(f)}";
                ParameterTypeModel paramModel = ParameterTypeList.First(_ => _.name.Replace(DataSource.ParamPrefixStr, string.Empty)
                                                        == argName.Replace(DataSource.ParamPrefixStr, string.Empty));//定義テーブルにはパラメータ頭文字は付けない想定だが念のためリプレイスしておく);
                if (DataSource.IsTableArgument(argDataType))
                {
                    CreateTableParam(ParameterTypeList, param, paramModel, argName, r, tran);
                }
                else
                {
                    //テーブル型以外の引数をparamに追加する
                    _ = r.ArgumentValues.TryGetValue(argName, out ArgumentValue? argVal);
                    if (argVal is null)
                    {
                        LogTo.Warning($"argVal is null. argName:[{argName}]");
                        //TODO エラー処理
                        continue;
                    }
                    Type? reqValType = Type.GetType(argDataType);
                    if (reqValType is null)
                    {
                        LogTo.Warning($"reqValType is null. argName:[{argName}]");
                        //TODO エラー処理
                        continue;
                    }
                    // 文字列からデータタイプの値に変換
                    object? value = reqValType != typeof(string) && string.IsNullOrEmpty(argVal.Value.ToString())
                        ? DBNull.Value
                        : Convert.ChangeType(argVal.Value.ToString(), reqValType);
                    //paramに値を追加。
                    param.Add(argName
                        , DataSource.CreateParamList(new Dictionary<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)> {
                    {argName ,(value , paramModel.max_length,DataSource.GetType(paramModel.type) , paramModel.is_output ? ParameterDirection.Output : ParameterDirection.Input) }
                    }).SingleOrDefault());//1件のみのはずなのでSingleOrDefaultで取得
                }
            }
            //TODO ログは役目が終わったら削除
            //string paramLog = "paramLog ";
            //string sqlDebugParamLog = string.Empty;
            //foreach (KeyValuePair<string, object?> p in param)
            //{
            //    try
            //    {
            //        paramLog += $",p.KeyVal:[{p.Key}]:[{(p.Value as IDbDataParameter)!.Value}]";
            //        //ストアドデバッグ時にコピペで張り付けたいため。ただし文字列は'をつける必要あり
            //        sqlDebugParamLog += $"SET @{p.Key} = {(p.Value as IDbDataParameter)!.Value};";
            //    }
            //    catch (Exception ex)
            //    {
            //        LogTo.Fatal(ex.Message);
            //    }
            //}
            //LogTo.Information(paramLog);
            //LogTo.Information(sqlDebugParamLog);

            LogTo.Debug($"{CreateParameterLogString(param)}");

            if (f.IS_FUNCTION_RETURN)
            {
                //ストアド実行
                ret = tran is null
                    ? ((int retCode, string retMsg))DataSource.ExecuteStoredFunctionReturnOretAndOmsg(f.FUNCTION_NAME, param, timeout: f.ProgramTimeOutSecond)
                    : ((int retCode, string retMsg))DataSource.ExecuteStoredFunctionReturnOretAndOmsg(tran.Connection, f.FUNCTION_NAME, param, tran, timeout: f.ProgramTimeOutSecond);
                LogTo.Information($"Excute_Complete:ExecuteStoredFunctionReturnOretAndOmsg");
                //以下、OUTPUTを取得しようとしたが、うまくいかなかった。定義テーブルとかみ合わないためコメント化。やるとしたら、ExecResultにOUTPUTを戻す用の仕組みを検討必要。
                //_ = DataSource.ExecuteStoredFunction(f.FUNCTION_NAME, param);//ref
                //SqlParameter objRetCode = (SqlParameter)(param["O_RET"] ?? throw new NullReferenceException());
                //SqlParameter objRetMsg = (SqlParameter)(param["O_MSG"] ?? throw new NullReferenceException());
                //ret = ((int)objRetCode.Value, (string)objRetMsg.Value);
            }
            else
            {
                //ストアド実行
                _ = tran is null
                    ? DataSource.ExecuteStoredFunction(f.FUNCTION_NAME, param, timeout: f.ProgramTimeOutSecond)
                    : DataSource.ExecuteStoredFunction(tran.Connection, f.FUNCTION_NAME, param, tran, timeout: f.ProgramTimeOutSecond);
                LogTo.Information($"Excute_Complete:ExecuteStoredFunction");
                ret = (successRetCode, string.Empty);
            }
            return ret;
        }
        catch (Exception ex)
        {
            //LogTo.Error(ex, "");
            throw;
        }
    }

    /// <summary>
    /// Tableパラメータ処理の切り出し
    /// </summary>
    private static void CreateTableParam(
        IEnumerable<ParameterTypeModel> ParameterTypeList, 
        Dictionary<string, object?> param,
        ParameterTypeModel paramModel,
        string argName,
        RequestValue r,
        IDbTransaction tran)
    {
        //DataSetを作成してPramにAddする

        //ユーザーテーブル型のみ抽出する //1ストアドに2つ以上の要素はあり得ない思想。
        var array = ParameterTypeList.Where(_ =>
                                        _.name.Replace(DataSource.ParamPrefixStr, string.Empty)
                                            == argName.Replace(DataSource.ParamPrefixStr, string.Empty)//定義テーブルにはパラメータ頭文字は付けない想定だが伝のためリプレイスしておく
                                            && _.is_user_defined == true
                                        ).ToArray();
        foreach (var ut in array) 
        {
            CreateTableParam2(ParameterTypeList, param, paramModel, argName, r, ut, tran);
        }
    }

    /// <summary>
    /// ユーザーテーブル型の引数１つ分を作成する
    /// </summary>
    /// <param name="ParameterTypeList"></param>
    /// <param name="param"></param>
    /// <param name="paramModel"></param>
    /// <param name="argName"></param>
    /// <param name="r"></param>
    /// <param name="ut"></param>
    /// <param name="tran"></param>
    private static void CreateTableParam2(
        IEnumerable<ParameterTypeModel> ParameterTypeList,
        Dictionary<string, object?> param,
        ParameterTypeModel paramModel,
        string argName,
        RequestValue r,
        ParameterTypeModel ut,
        IDbTransaction tran)
    {
        //ユーザーテーブル型からカラム情報を取得する
        string[] paramName = [ "UserTableTypeName" ];
        var query = DataSource.CreateTableFunctionQuery("GetObjectTableTypeColumnsDefine", paramName, [("ColumnId", false)]);
        var paramTable = DataSource.CreateParamTable(new() 
                {
                    { "UserTableTypeName", (ut.type , 128 , typeof(string),null) }
                });
        var userTableTypeColumnList = tran is null
            ? DataSource.GetEntityCollection<UserTableTypeColumnModel>(query, paramTable)
            : DataSource.GetEntityCollection<UserTableTypeColumnModel>(tran.Connection, query, paramTable, tran);
        if (r.ArgumentDataSet.Count == 1)
        {
            //従来と同様の動作
            CreateTableParam3(param, paramModel, argName, ut, userTableTypeColumnList, r.ArgumentDataSet.First());
        }
        else
        {
            foreach (var rrr in r.ArgumentDataSet.Where(m => m.Key == argName))
            {
                CreateTableParam3(param, paramModel, argName, ut, userTableTypeColumnList, rrr);
            }
        }
    }


    private static void CreateTableParam3(
        Dictionary<string, object?> param,
        ParameterTypeModel paramModel,
        string argName,
        ParameterTypeModel ut,
        IEnumerable<UserTableTypeColumnModel> userTableTypeColumnList,
        KeyValuePair<string, SortedList<int, List<ArgumentValue>>> rrr)
    {
        {
            // DataTableのインスタンスを作成
            DataTable dt = new();
            //カラム情報をDataTable dtに追加する
            foreach (var utColumn in userTableTypeColumnList)
            {
                //_ = dt.Columns.Add(utColumn.ColumnName);//TODO カラムタイプを.netのtypeで指定したい
                _ = dt.Columns.Add(utColumn.ColumnName, DataSource.GetType(utColumn.DataType));
            }

            //dtにArgumentDataSetを追加する
            foreach (var reqRow in rrr.Value)
            {
                //行objを作成
                DataRow dtRow = dt.NewRow();
                //int key = reqRow.Key;
                List<ArgumentValue> argValues = reqRow.Value;
                foreach (UserTableTypeColumnModel utColumn in userTableTypeColumnList)
                {
                    ArgumentValue argval = argValues.First(_ => _.ArgumentName == utColumn.ColumnName);//nullの場合はエラーにしたいためFirstで取得
                                                                                                       //dtRow[utColumn.ColumnName] = argval.Value;
                    object? value = null;
                    Type? reqValType = DataSource.GetType(utColumn.DataType);
                    value = reqValType != typeof(string) && string.IsNullOrEmpty(argval.Value.ToString()) && utColumn.IsNullable
                        ? DBNull.Value
                        : Convert.ChangeType(argval.Value.ToString(), reqValType);
                    dtRow[utColumn.ColumnName] = value;

                }
                //dtに行objを追加
                dt.Rows.Add(dtRow);
            }
            //paramにデータテーブル型変数を追加する。
            param.Add(ut.name,
                DataSource.CreateParamList(new() {
                    {argName ,(dt, paramModel.max_length, typeof(DataTable) , paramModel.is_output ? ParameterDirection.Output : ParameterDirection.Input) }
            }).SingleOrDefault()
                ); // DataTable型でIDbDataParameterを作成しないといけないのではないか。  //typeはかならず.net DataTableとしておく。
        }
    }

    private static string CreateParameterLogString(Dictionary<string, object?> param)
    {
        StringBuilder sb = new();
        sb.AppendLine("stored function parameter : [");
        foreach (KeyValuePair<string, object?> p in param)
        {
            try
            {
                sb.AppendLine($"{p.Key} = 「{(p.Value as IDbDataParameter)!.Value}」");
            }
            catch (Exception ex)
            {
                LogTo.Fatal(ex, "パラメータログ生成で例外が発生");
            }
        }
        sb.AppendLine("]");
        return sb.ToString();
    }

    /// <summary>
    /// ファンクション名と実行パスからバッチファイルを実行する
    /// </summary>
    /// <param name="f"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    private static (int retCode, string retMsg) ExecuteOuterFileProcess(ProcessFunctionModel f, RequestValue r)
    {
        (int retCode, string retMsg) ret = (int.MinValue, string.Empty);
        //ProcessFunctionModel fのFUNCTION_NAMEとEXEC_PATHを使用して、バッチファイルを実行する。

        //コマンドライン引数を作成
        string param = string.Empty;
        for (int i = 0; i < f.ARGUMENT_COUNT; i++)
        {
            PropertyInfo? propertyInfo = f.GetType().GetProperty($"ARGUMENT_NAME{i + 1}");
            if (propertyInfo is null)
            {
                LogTo.Warning($"propertyInfo is null");
                //TODO エラー処理
                continue;
            }
            string argName = $"{propertyInfo.GetValue(f)}";
            _ = r.ArgumentValues.TryGetValue(argName, out ArgumentValue? argVal);
            if (argVal is null)
            {
                LogTo.Warning($"argVal is null");
                //TODO エラー処理
                continue;
            }
            //コマンドライン引数文字列として作成
            param += $" {argVal.Value}";
        }

        string execTargetFilePath = f.EXEC_TARGET_PATH;//絶対パスで指定されているとする
        if (string.IsNullOrWhiteSpace(execTargetFilePath))
        {
            LogTo.Warning($"string.IsNullOrWhiteSpace(execTargetFilePath) is true");
            //TODO エラー処理
            return ret;
        }
        // プロセスを作成して実行
        Process process = new();
        // バッチファイルの実行か、powershell実行によって分ける
        if (execTargetFilePath.Contains(".ps1"))
        {
            //TODO 要確認
            process.StartInfo.FileName = "powershell.exe";//絶対パスでなくてよい確認する
            process.StartInfo.Arguments = $"-File {execTargetFilePath}{param}";
        }
        else
        {
            //batファイルやexeファイルの実行設定
            process.StartInfo.FileName = execTargetFilePath;
            process.StartInfo.Arguments = param;
        }
        LogTo.Information(
            $"process.StartInfo.FileName:[{process.StartInfo.FileName}]" +
            $",process.StartInfo.Arguments:[{process.StartInfo.Arguments}]"
            );
        //実行
        _ = process.Start();
        // 終了コードを取得
        int exitCode = 0;
        //TODO 実行できなかった場合の処理
        //TODO ゆくゆくは非同期フラグを持たせて、処理を待つか分ける。一旦、IS_RETURNで判断するとする
        if (f.IS_FUNCTION_RETURN)
        {
            // プロセスが終了するまで待機
            process.WaitForExit();
            exitCode = process.ExitCode;
        }

        //ログ
        LogTo.Information($"process.ExitCode:[{exitCode}]");
        //TODO retのメッセージが必要
        ret = (exitCode, string.Empty);
        return ret;
    }

}
