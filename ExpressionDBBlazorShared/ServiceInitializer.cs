using BlazorDownloadFile;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using ExpressionDBBlazorShared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sotsera.Blazor.Toaster.Core.Models;

namespace ExpressionDBBlazorShared;
public static class ServiceInitializer
{
    public static void AddSharedServices(this IServiceCollection services, IConfiguration config)
    {
        // AddSingleton : 全てのリクエストに対して共通インスタンス
        // AddScoped : セッションごとに異なるインスタンス(WASMではsingletonと同じ挙動)
        // AddTransient : ページ遷移ごとに新しいインスタンス

        //DI inject
        _ = services.AddScoped<ChildBaseService>();
        _ = services.AddScoped<SystemParameterService>();
        _ = services.AddScoped<HtService>();
        _ = services.AddScoped<WebAPIService>();
        _ = services.AddScoped<CommonService>();
        _ = services.AddScoped<CustomForSuntoryService>();
        _ = services.AddScoped<CommonWebComService>();
        _ = services.AddScoped<DeviceInfoService>();
        _ = services.AddScoped<ChatService>();
        _ = services.AddSingleton(new ApplicationVersion
        {
            Version = config.GetValue<string>("ApplicationVersion") ?? string.Empty
        });

        _ = services.AddToaster(o =>
        {
            o.PositionClass = Defaults.Classes.Position.TopRight;
            o.MaximumOpacity = 100;
            o.VisibleStateDuration = 1000 * 5;
            o.ShowTransitionDuration = 10;
            o.HideTransitionDuration = 500;
            o.PreventDuplicates = true;//複数表示させるかどうか。
            o.NewestOnTop = true;
        });

        //Radzen 
        _ = services.AddScoped<DialogService>();
        _ = services.AddScoped<NotificationService>();
        _ = services.AddScoped<TooltipService>();
        _ = services.AddScoped<ContextMenuService>();

        //ローカルストレージを追加。多重ログイン管理に使用する。
        _ = services.AddBlazoredLocalStorage();
        _ = services.AddBlazoredSessionStorage();

        //Blazor DownLoadFile
        _ = services.AddBlazorDownloadFile(ServiceLifetime.Scoped);

        //string cultureName = CultureInfo.CurrentCulture.Name;
        //if (cultureName == "ja")
        //{
        //    cultureName = "ja-JP";
        //}
        //CultureInfo cultureInfo = new(cultureName);
        //CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        //CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }
}