using ExpressionDBBlazorShared;
using ExpressionDBBlazorWasmApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Text;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using BlazorA11yDemo.Client;


Console.OutputEncoding = Encoding.UTF8;
WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Main>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ローディングバーを追加
builder.Services.AddLoadingBarService();

//Sharedプロジェクト側のサービスを追加する
builder.Services.AddSharedServices(builder.Configuration);

builder.Services.AddSingleton<IPlatformNameProvider, PlatformNameProvider>();

// MenuServiceでHttpClientを使用したいためAddSingletonに変更
// MenuServiceのWebAPIへの問合せ部分はWebAPIServiceクラスに行わせるように変更する
// その後はAddScodeに戻しても良い

//ローディングバーを使用する
builder.UseLoadingBar();

// appsettings.jsonからBaseUriを読み込む
string baseUri = builder.Configuration.GetValue<string>("ConnectionStrings:BaseAddressUri") ?? throw new NullReferenceException();
builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(baseUri) }
.EnableIntercept(sp));//ローディングバー有効

// IHttpContextAccessorを使用する。（IPアドレス取得用）
builder.Services.AddHttpContextAccessor();

//builder.Services.AddLanguageContainer(Assembly.GetExecutingAssembly());


await builder.Build().RunAsync();
