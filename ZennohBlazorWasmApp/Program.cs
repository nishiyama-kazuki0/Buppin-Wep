using BlazorDownloadFile;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using ZennohBlazorShared;
using ZennohBlazorShared.Services;
using ZennohBlazorWasmApp;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Main>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// AddSingleton : 全てのリクエストに対して共通インスタンス
// AddScoped : セッションごとに異なるインスタンス(WASMではsingletonと同じ挙動)
// AddTransient : ページ遷移ごとに新しいインスタンス

//DI inject
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

// MenuServiceでHttpClientを使用したいためAddSingletonに変更
// MenuServiceのWebAPIへの問合せ部分はWebAPIServiceクラスに行わせるように変更する
// その後はAddScodeに戻しても良い

// appsettings.jsonからBaseUriを読み込む
string baseUri = builder.Configuration.GetValue<string>("ConnectionStrings:BaseAddressUri") ?? throw new NullReferenceException();
builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(baseUri) });

//ローカルストレージを追加。多重ログイン管理に使用する。
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();

// IHttpContextAccessorを使用する。（IPアドレス取得用）
builder.Services.AddHttpContextAccessor();

builder.Services.AddBlazorDownloadFile(ServiceLifetime.Scoped);

//builder.Services.AddScoped(sp =>
//    new DbManageService(
//        builder.Configuration.GetConnectionString("AppDBConnection")
//        , MultipleDatabaseAccessor.Enumerators.EnumDatabaseType.SqlServer
//        , Convert.ToInt32(builder.Configuration.GetConnectionString("Timeout"))
//        )
//);
//builder.Services.AddLanguageContainer(Assembly.GetExecutingAssembly());

//builder.Services.AddSingleton<IJSRuntime>(provider =>
//    provider.GetRequiredService<IJSRuntime>());
//var jsRuntime = builder.Services.BuildServiceProvider()
//    .GetRequiredService<IJSRuntime>();
//await jsRuntime.InvokeVoidAsync("window.addEventListener", "keydown", DotNetObjectReference.Create(new KeyEventHandler()));
await builder.Build().RunAsync();
