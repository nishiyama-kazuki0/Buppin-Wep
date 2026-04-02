using Serilog;
using ZennohCycleProcessApp;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // IConfigurationを作成
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        //ログ出力準備
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        // appsettings.jsonから設定を読み込む
        string baseUrl = configuration["ConnectionStrings:BaseAddressUri"] ?? throw new NullReferenceException();
        _ = services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(baseUrl) });
        _ = services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
