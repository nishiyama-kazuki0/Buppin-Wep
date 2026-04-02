using Anotar.Serilog;
using Newtonsoft.Json;
using SharedModels;
using System.Net.Http.Json;
using System.Text;

namespace ExpressionDBCycleProcessApp;

/// <summary>
/// WEBAPIを利用して定周期処理を実行するバックグラウンドサービス
/// １．「定周期処理初期化」を実行後
/// ２．SQLServerのGetCycleProcessState()を呼び出して、DEFINE_CYCLE_PROCESSの定義を元にCycleProcessInfoのリストを取得
/// ３．upc_UPDATE_CYCLE_PROCESS_STATEを呼び出して、処理中とし、
/// 　　処理実行後、再度upc_UPDATE_CYCLE_PROCESS_STATEを呼び出して、処理中フラグを落とす。
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// HttpClient
    /// </summary>
    private readonly HttpClient _httpClient;
    private readonly int _cycleInterval;
    private readonly int _httpComTimeout;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    /// <param name="httpClient"></param>
    public Worker(ILogger<Worker> logger, IConfiguration configuration, HttpClient httpClient)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClient;
        _httpComTimeout = (int)httpClient.Timeout.TotalMilliseconds;
        _cycleInterval = configuration.GetValue<int>("CycleInterval");
        _logger.LogInformation($"constructor_cycleInterval:[{_cycleInterval}],_httpComTimeout:[{_httpComTimeout}]");
    }

    /// <summary>
    /// IHostedServiceとして実行すべきタスクの定義
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //周期処理定義の初期化を行うストアドを最初に一度呼び出しておく。万が一前回が処理途中でアプリを終了されたときに復旧できるように備えるため。
        _ = await RequestCycleProcessInitializeAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            //確認

            await Task.Delay(_cycleInterval, stoppingToken);//appjsonファイルで設定している周期

            //_httpClientを使用してCycleProcessControllerのGetを呼びだし、
            //CycleProcessInfoのリストを取得する
            IEnumerable<CycleProcessInfo>? infoList = null;
            try
            {
                infoList = await _httpClient.GetFromJsonAsync<IEnumerable<CycleProcessInfo>>("CycleProcess");//[ApiController]属性を付けているので、\Getの指定は省略とのこと
            }
            catch (Exception ex)
            {
                LogTo.Fatal($"Error1{ex.Message}");
                continue;
            }

            if (infoList is null)
            {
                //TODO エラー処理
                LogTo.Warning($"infoList is null");
                continue;
            }
            else
            {
                LogTo.Warning($"infoList.Any():{infoList.Any()}");
            }

            // ここでCycleProcessDefine情報を取得して処理を行う。
            foreach (CycleProcessInfo info in infoList)
            {
                //現在実行中のプロセスがある場合は処理を行わない。(SELECTのWHEREでも開始時間、フラグを見ているが、念のためここでも絞る)
                if (info.IS_PROCESSING || info.INTERVAL <= 0)
                {
                    continue;
                }

                RequestValue reqStarted = RequestValue.CreateRequestProgram("定周期処理実行開始")
                    .SetArgumentValue("CYCLE_ID", info.CYCLE_ID, "System.Int16")
                    .SetArgumentValue("CATEGORY", info.CATEGORY, "System.Int16")
                    .SetArgumentValue("OBSERVE_ID", info.OBSERVE_ID, "System.Int16")
                    .SetArgumentValue("UPDATE_IS_PROCESSING", true, "System.Boolean")//実行中のフラグを立てる用
                    .SetArgumentValue("CLASS_NAME", GetType().Name, "System.String")
                    .SetArgumentValue("USER_ID", "999999", "System.String");//TODO 通知ログで必ず必要なので追加しておく//サーバー常駐は一旦NULL指定
                ;
                //実行中フラグを立てる//実行フラグを落とすのは、ファンクション処理の最後で行う
                string json = JsonConvert.SerializeObject(reqStarted);
                StringContent reqUpdateContent = new(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                try
                {
                    response = await _httpClient.PostAsync("/Execution", reqUpdateContent);
                }
                catch (Exception ex)
                {
                    LogTo.Fatal($"Error2:{ex.Message}");
                    continue;
                }

                try
                {
                    if (!await CheckResponseResultAsync(response))
                    {
                        continue;
                    }

                    //排他処理とするかどうか
                    if (info.IS_EXCLUSIVE)
                    {
                        //排他処理の場合は、完了するまで次のCycleProcessProgramは実行したくないので待たせる
                        _ = await RequestExecuteProcessProgramAsync(info, Timeout.InfiniteTimeSpan);
                    }
                    else
                    {
                        //排他処理ではない場合は、タスクで実行させてしまい、現在の処理が実行中でも次のループのCycleProcessProgramの実行判定を行う。
                        _ = Task.Run(() =>
                        {
                            _ = RequestExecuteProcessProgramAsync(info, TimeSpan.FromMilliseconds(_httpComTimeout));
                        });
                    }
                }
                catch (Exception ex)
                {
                    LogTo.Fatal($"Error3:{ex.Message}");
                    continue;
                }


            }
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //LogTo.Information($"Worker running at: {DateTimeOffset.Now}");
        }

        //インナーメソッド プロセスプログラムに定義されいてる周期処理を実行する
        async Task<bool> RequestExecuteProcessProgramAsync(CycleProcessInfo info, TimeSpan timeoutValue)
        {
            //infoはこのタスクの引数に渡さなくてもこのタスクにコピーされるとのこと。大量にタスクが生成されると、
            //メモリをかなり使用してしまうが、実行中は新しいタスクを生成しないはずなので子の処理とする。
            //ここでWebAPIのExecutionControllerを呼び出す。
            RequestValue req = RequestValue.CreateRequestProgram(info.PROCESS_PROGRAM_NAME)
                .SetArgumentValue("CYCLE_ID", info.CYCLE_ID, "System.Int16")
                .SetArgumentValue("CATEGORY", info.CATEGORY, "System.Int16")
                .SetArgumentValue("TARGET_TABLE_NAME", info.TARGET_TABLE_NAME, "System.String")
                .SetArgumentValue("TARGET_COLUMNS_NUM", info.TARGET_COLUMNS_NUM, "System.Int32")
                .SetArgumentValue("TARGET_PATH", info.TARGET_PATH, "System.String")
                .SetArgumentValue("BACK_UP_PATH", info.BACK_UP_PATH, "System.String")
                .SetArgumentValue("TARGET_FILE_NAME", info.TARGET_FILE_NAME, "System.String")
                .SetArgumentValue("START_DATETIME", info.START_DATETIME, "System.DateTime")
                .SetArgumentValue("SEPSTR", info.SEPSTR, "System.String")
                .SetArgumentValue("OBSERVE_ID", info.OBSERVE_ID, "System.Int16")
                .SetArgumentValue("HEADER_ENABLE", info.HEADER_ENABLE, "System.Boolean")
                .SetArgumentValue("UPDATE_IS_PROCESSING", false, "System.Boolean")
                .SetArgumentValue("CLASS_NAME", GetType().Name, "System.String")
                .SetArgumentValue("USER_ID", "999999", "System.String")//TODO 通知ログで必ず必要なので追加しておく//サーバー常駐は一旦NULL指定
                ;

            //実行中フラグを落とすように引数を調整する
            string json = JsonConvert.SerializeObject(req);
            StringContent reqContent = new(json, Encoding.UTF8, "application/json");// タイムアウトを設定する 
            CancellationTokenSource cts =
                timeoutValue == Timeout.InfiniteTimeSpan ?
                new CancellationTokenSource() //無制限
                : new CancellationTokenSource(timeoutValue);
            HttpResponseMessage response = await _httpClient.PostAsync("/Execution", reqContent, cts.Token);
            bool retb = await CheckResponseResultAsync(response);
            if (!retb)
            {
                LogTo.Fatal($"Error3:PROCESS_PROGRAM_NAME:{info.PROCESS_PROGRAM_NAME},retb:{retb}");
                //失敗しているなら実行中のフラグを落とすプログラムをリクエストする
                RequestValue reqStarted = RequestValue.CreateRequestProgram("定周期処理実行開始")//実行開始だがフラグを落とす要求
                    .SetArgumentValue("CYCLE_ID", info.CYCLE_ID, "System.Int16")
                    .SetArgumentValue("CATEGORY", info.CATEGORY, "System.Int16")
                    .SetArgumentValue("OBSERVE_ID", info.OBSERVE_ID, "System.Int16")
                    .SetArgumentValue("UPDATE_IS_PROCESSING", false, "System.Boolean")//実行中のフラグを落とす
                    .SetArgumentValue("CLASS_NAME", GetType().Name, "System.String")
                    .SetArgumentValue("USER_ID", "999999", "System.String");//TODO 通知ログで必ず必要なので追加しておく//サーバー常駐は一旦NULL指定
                                                                            //実行中フラグを立てる//実行フラグを落とすのは、ファンクション処理の最後で行う
                string jRecover = JsonConvert.SerializeObject(reqStarted);
                StringContent reqUpdateContent = new(jRecover, Encoding.UTF8, "application/json");
                HttpResponseMessage responseRecover;
                try
                {
                    responseRecover = await _httpClient.PostAsync("/Execution", reqUpdateContent);
                }
                catch (Exception ex)
                {
                    LogTo.Fatal($"Error4:{ex.Message}");
                }
            }
            return retb;
        }

        //インナーメソッド レスポンスexecresultが成功しているか確認します。
        async Task<bool> CheckResponseResultAsync(HttpResponseMessage response)
        {
            ExecResult[]? resItems = null;
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Failure:response.IsSuccessStatusCode: {response.IsSuccessStatusCode}", response.IsSuccessStatusCode);
                LogTo.Warning($"response.IsSuccessStatusCode:{response.IsSuccessStatusCode}");
                //ここでこの処理を抜ける
                return false;
            }
            _logger.LogInformation("OK:response.IsSuccessStatusCode: {response.IsSuccessStatusCode}", response.IsSuccessStatusCode);
            resItems = await response.Content.ReadFromJsonAsync<ExecResult[]>();
            if (resItems is null)
            {
                _logger.LogInformation("resItems ExecResults is null");
                LogTo.Warning($"resItems ExecResults is null");
                //ここでこのタスクを抜ける
                return false;
            }
            if (resItems.Min(_ => _.RetCode) < 0)
            {
                LogTo.Warning($"resItems RetCode:Error");
                return false;
            }

            foreach (ExecResult o in resItems)
            {
                _logger.LogInformation($"execResult " +
                    $"ProgramName:{o.ProgramName}" +
                    $",FunctionName:{o.FunctionName}" +
                    $",ExecOrderRank:{o.ExecOrderRank}" +
                    $",RetCode:{o.RetCode}" +
                    $",Message{o.Message}"
                    );
                LogTo.Information($"execResult " +
                    $"ProgramName:{o.ProgramName}" +
                    $",FunctionName:{o.FunctionName}" +
                    $",ExecOrderRank:{o.ExecOrderRank}" +
                    $",RetCode:{o.RetCode}" +
                    $",Message{o.Message}"
                    );
            }

            return true;
        }
    }
    /// <summary>
    /// 初期化ストアド実行をリクエストするメソッド。周期のwhileループに入る前に一度だけ実行されるとする。
    /// </summary>
    /// <returns></returns>
    private async Task<bool> RequestCycleProcessInitializeAsync()
    {
        await Task.Delay(0);
        RequestValue reqStarted = RequestValue.CreateRequestProgram("定周期処理初期化")
                    .SetArgumentValue("CLASS_NAME", GetType().Name, "System.String")
                    .SetArgumentValue("USER_ID", "999999", "System.String");//TODO 通知ログで必ず必要なので追加しておく//サーバー常駐は一旦NULL指定
        ;
        //実行中フラグを立てる//実行フラグを落とすのは、ファンクション処理の最後で行う
        string json = JsonConvert.SerializeObject(reqStarted);
        StringContent reqUpdateContent = new(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsync("/Execution", reqUpdateContent);
        }
        catch (Exception ex)
        {
            LogTo.Fatal($"Error_INITIL_PROC:{ex.Message}");
        }

        return true;
    }
}