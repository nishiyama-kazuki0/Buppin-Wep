using ExpressionDBCycleProcessApp;
using ExpressionDBCycleProcessApp.Maintenance;
using ExpressionDBCycleProcessApp.MELSECComm;
using ExpressionDBCycleProcessApp.MELSECComm.SequenceProcess;
using ExpressionDBCycleProcessApp.ViewModels;
using ExpressionDBCycleProcessApp.Views;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using System.Runtime.InteropServices;
using System.Text;

//(サーバーOSでもエラーを発生させずに動作させるため)Stylus（ペン）とタッチサポートを無効化する
AppContext.SetSwitch("Switch.System.Windows.Input.Stylus.DisableStylusAndTouchSupport", true);

//SJISエンコーディングを使用するための設定
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
//コンソールを割り当てる（デバッグ用）
ConsoleManager.AllocConsole();
Console.OutputEncoding = Encoding.UTF8;

var builder = WpfApplication<App, MainWindow>.CreateBuilder(args);
{
    // IConfigurationを作成
    IConfigurationRoot configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();
    //ログ出力準備
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        //状態遷移のログ
        //エンコーディング指定はコードからしかできない様なので、ここで指定
        .WriteTo.Logger(cfg => cfg
            .Filter.ByIncludingOnly(Matching.FromSource("ExpressionDBCycleProcessApp.Maintenance.MaintenanceManager"))
            .WriteTo.Async(a => a.File(
                path: $"{MyConst.StepHistoryFolder}/{MyConst.StepHistoryFileBaseName}{MyConst.StepHistoryExt}",
                outputTemplate: "{Timestamp:HH:mm:ss.fff},{ThreadId:00},{Message:j}{NewLine}",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 10485760,
                retainedFileCountLimit: 10,
                buffered: false,
                restrictedToMinimumLevel: LogEventLevel.Verbose,
                encoding: Encoding.GetEncoding("shift_jis") // ← もしShift_JISで出したい場合
            ))
        )
        .CreateLogger();

    //すべての例外をキャッチする
    AppDomain.CurrentDomain.FirstChanceException += (sender, args) =>
    {
        var ex = args.Exception;
        Anotar.Serilog.LogTo.Error(ex, "FirstChanceException: {Message}", ex.Message);
    };

    // appsettings.jsonから設定を読み込む
    string baseUrl = configuration["ConnectionStrings:BaseAddressUri"] ?? throw new NullReferenceException();

    //HttpClientの生成と初期設定
    HttpClient hc = new(new HttpClientHandler { MaxConnectionsPerServer = 1000 }) { BaseAddress = new Uri(baseUrl) };
    var httpComTimeout = configuration.GetValue<int>("HttpComTimeout");
    hc.Timeout = TimeSpan.FromMilliseconds(httpComTimeout);

    //ソースフォルダを取得
    MaintenanceManager.SourceFolder = configuration.GetValue<string>("SourceFolderPath");

    //依存性注入の設定
    builder.Services
        .AddSingleton(sp => hc)
        .AddSingleton<MaterialHandlingManager>()
        .AddSingleton<MaintenanceManager>()
        .AddTransient<MainWindowViewModel>()
        .AddTransient<UcSequenceActionsViewModel>()
        .AddTransient<UcSequenceActions2ViewModel>()
        .AddHostedService<Worker>()
        ;
}
var app = builder.Build();

//メンテナンスマネージャーの初期化
var mm = app.Services.GetRequiredService<MaintenanceManager>();
mm.InitCommunicationAreaViewModels(app.Services);

SequenceAction.OnStepChanged = MaintenanceManager.OnStepChanged;

var o = app.Services.GetRequiredService<MaterialHandlingManager>();

//先にSequenceActionViewModelを生成させる
MaintenanceManager.Root = SequenceActionViewModel.GetOrCreateSequenceActionViewModel(app.Services, o.CommTask.seqManager, false);

//デバイス転送のテスト呼び出し
//var flag = false;
//if (flag)
//{
//    Task.Run(async () =>
//    {
//        byte[] binaryData = new byte[49800];
//        new Random().NextBytes(binaryData); // ダミーデータ
//        await o.CommTask.seqManager.HandlingMngr.CmnWebApi.SendBynaryData(binaryData);
//    });
//}

//PLCとの通信を開始する
o.StartCommunication();

app.Run();

/// <summary>
/// コンソールを割り当てるためのクラス
/// </summary>
class ConsoleManager
{
    [DllImport("kernel32.dll")]
    public static extern bool AllocConsole();
}
