using BlazorDownloadFile;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.Extensions.Configuration;
using Radzen;

using ZennohBlazorShared;
using ZennohBlazorShared.Services;

namespace ZennohBlazorMauiApp;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        // AddSingleton : 全てのリクエストに対して共通インスタンス
        // AddScoped : セッションごとに異なるインスタンス(WASMではsingletonと同じ挙動)
        // AddTransient : ページ遷移ごとに新しいインスタンス

        //DIコンテナはここで定義する

        builder.Services.AddScoped<ChildBaseService>();
        builder.Services.AddScoped<HtService>();
        builder.Services.AddScoped<WebAPIService>();
        builder.Services.AddScoped<CommonService>();
        builder.Services.AddSingleton(new ApplicationVersion
        {
            Version = builder.Configuration.GetValue<string>("ApplicationVersion") ?? string.Empty
        });
        //Radzen 
        builder.Services.AddScoped<DialogService>();
        builder.Services.AddScoped<NotificationService>();
        builder.Services.AddScoped<TooltipService>();
        builder.Services.AddScoped<ContextMenuService>();

        builder.Services.AddSingleton<IPlatformNameProvider, PlatformNameProvider>();

        // appsettings.jsonからBaseUriを読み込む
        string baseUri = builder.Configuration.GetValue<string>("ConnectionStrings:BaseAddressUri") ?? throw new NullReferenceException();
        builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(baseUri) });

        //ローカルストレージを追加。多重ログイン管理に使用する。
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddBlazoredSessionStorage();

        // IHttpContextAccessorを使用する。（IPアドレス取得用）
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddBlazorDownloadFile(ServiceLifetime.Scoped);

        //TODO
        //must specify the host base address as the mauibuilder do not have hostenvironment extension
        //HttpClientHandler handler = new()
        //{
        //    UseDefaultCredentials = false,
        //    Credentials = System.Net.CredentialCache.DefaultCredentials,
        //    AllowAutoRedirect = true
        //};
        //builder.Services.AddScoped(sp =>
        //new HttpClient(handler)
        //{
        //    BaseAddress = new Uri("http://127.0.0.1")
        //});

        //TODO 確認
        //using Stream stream = Assembly.GetExecutingAssembly()
        //                                        .GetManifestResourceStream($"{typeof(MauiProgram).Namespace}.appsettings.json");//埋め込みリソースである必要あり
        //IConfigurationRoot config = new ConfigurationBuilder().AddJsonStream(stream).Build();
        //builder.Configuration.AddConfiguration(config);

        //出来なかった
        //builder.Services.AddScoped<JsonFileReader>();
        ////set up the appsetting json file as embeded resource
        //using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;
        //var config = new ConfigurationBuilder()
        //.AddJsonStream(stream)
        //.Build();
        //builder.Configuration.AddConfiguration(config);

        //builder.Services.AddScoped(sp =>
        //new DbManageService(
        //        builder.Configuration.GetConnectionString("AppDBConnection")
        //        , MultipleDatabaseAccessor.Enumerators.EnumDatabaseType.SqlServer
        //        , Convert.ToInt32(builder.Configuration.GetConnectionString("Timeout"))
        //    ));
        //builder.Services.AddLanguageContainer(Assembly.GetExecutingAssembly());
        return builder.Build();
    }
}
