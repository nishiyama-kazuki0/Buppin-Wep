using Microsoft.AspNetCore.SignalR;
using Radzen;
using ZennohBlazorServerApp;
using ZennohBlazorShared;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient();

// AddSingleton : 全てのリクエストに対して共通インスタンス
// AddScoped : セッションごとに異なるインスタンス(WASMではsingletonと同じ挙動)
// AddTransient : ページ遷移ごとに新しいインスタンス

//DI inject
//builder.Services.AddSingleton<SharedService>();
//builder.Services.AddSingleton<LoginService>();
//builder.Services.AddSingleton<MenuService>();

//Radzen 
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

builder.Services.AddSingleton<IPlatformNameProvider, PlatformNameProvider>();

//32Kbを超える通信を許可する
builder.Services.Configure<HubOptions>(options =>
{
    options.MaximumReceiveMessageSize = null;
});

//builder.Services.AddScoped(sp =>
//    new DbManageService(
//        builder.Configuration.GetConnectionString("AppDBConnection")
//        , MultipleDatabaseAccessor.Enumerators.EnumDatabaseType.SqlServer
//        , Convert.ToInt32(builder.Configuration.GetConnectionString("Timeout"))
//        )
//);
//builder.Services.AddLanguageContainer(Assembly.GetExecutingAssembly());

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    _ = app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(userEndPpoints =>
{
    _ = userEndPpoints.MapControllers();
});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
