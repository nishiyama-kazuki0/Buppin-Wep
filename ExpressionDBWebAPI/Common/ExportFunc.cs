using Anotar.Serilog;
using CsvHelper;
using CsvHelper.Configuration;
using ExpressionDBWebAPI.Controllers;
using SharedModels;
using System.Globalization;
using System.Text;

namespace ExpressionDBWebAPI.Common;

public class ExportFunc
{
    public static (int retCode, string retMsg) データファイル出力()
    {
        (int retCode, string retMsg) ret = (0, string.Empty);
        List<Task> taskList = [];
        try
        {
            //DEFINE_CYCLE_PROCESSからファイル出力の定義を取得する
            IEnumerable<CycleProcessInfo> infoList = CycleProcessController.GetCycleProcessInfo(category: 2); //データ出力に関するカテゴリーは2とする
            foreach (CycleProcessInfo infop in infoList)
            {

                // 定周期定義でIS_EXCLISUICVBEがfalseの場合は、非同期実行とする。
                //原則、本日作業分のデータを出力するので、非同期として問題ない。
                //データの絞り込みは、前回日締め日時から、現在日時までのデータを出力とする。※VIEW内で定義してしまう
                //テーブルごとに異なるなら、VIEWなどで埋め込んでしまい、SELECTするのみとする。
                //その場合は、定周期処理に定義されているテーブル名は、VIEW名となる。

                //排他処理とするかどうか
                if (infop.IS_EXCLUSIVE)
                {
                    //排他処理の場合は、完了するまで次のCycleProcessProgram(データ出力)は実行したくないので待たせる
                    _ = ExportByCSVHelperAsync(infop);
                }
                else
                {
                    //排他処理ではない場合は、タスクで実行させてしまい、現在の処理が実行中でも次のループのCycleProcessProgramの実行判定を行う。
                    taskList.Add(Task.Run(() =>
                    {
                        _ = ExportByCSVHelperAsync(infop);
                    }));
                }
            }
            if (taskList.Count > 0)//念のためカウントをとる
            {
                _ = Task.WhenAll(taskList);
            }
        }
        catch (Exception ex)
        {
            LogTo.Fatal(ex.Message);
            return (-1, ex.Message);
        }
        return ret;

        //インナーメソッド
        static async Task<bool> ExportByCSVHelperAsync(CycleProcessInfo info)
        {
            await Task.Delay(0);
            //ファイル出力の定義に基づいて、データを取得する
            ClassNameSelect select = new()
            {
                viewName = info.TARGET_TABLE_NAME
                ,
                tsqlHints = EnumTSQLhints.NOLOCK
            };
            List<ResponseValue> resItems = CommonController.GetResponseValue(select);
            if (resItems.Count <= 0)
            {
                //TODO 必要であればエラー処理を行う
                //ログ
                LogTo.Warning($"データ出力対象のデータがありません。{info.TARGET_TABLE_NAME}");
                return false;
            }

            //ターゲットパスフォルダを確認し、存在しなければフォルダを作成する
            string tgpath = $@"{info.TARGET_PATH.Replace("{YYYYMMDD}", $"{DateTime.Now:yyyyMMdd}")}";
            if (!Directory.Exists(tgpath))
            {
                _ = Directory.CreateDirectory(tgpath);
            }

            //ファイルパスを作成する \は付与されていないフォルダパスが格納されているものとする。(エクスプローラーでそのままパスをコピーしたいため)
            string fileName = $@"{tgpath}\"
                                            + $"{DateTime.Now:yyyyMMddHHmmssfff}" //必ず日時を付与するなど、パラメータで可変にしたいところ
                                            + $"{info.TARGET_FILE_NAME}"//拡張子は含められているのものとする
                                            ;

            //CSVヘルパーでCSV出力を行う
            using StreamWriter writer = new(fileName, false, Encoding.GetEncoding("shift_jis"));
            using CsvWriter csv = new(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = info.SEPSTR
            });
            if (info.HEADER_ENABLE)
            {
                //ヘッダー有りなら出力する
                foreach (string column in resItems.First().Columns)
                {
                    csv.WriteField(column);
                }
                csv.NextRecord();
            }
            //データ部を出力する
            foreach (ResponseValue responseValue in resItems)
            {
                foreach (KeyValuePair<string, object> value in responseValue.Values)
                {
                    foreach (string column in responseValue.Columns)
                    {
                        if (value.Key == column)
                        {
                            csv.WriteField(value.Value);
                        }
                    }
                }
                csv.NextRecord();
            }
            return true;
        }

    }
}
