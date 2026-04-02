using Anotar.Serilog;
using AspireServiceDefaults;
using ExpressionDBWebAPI.Common;
using Serilog;
using System.Text;
using BlazorDownloadFile;
using ExpressionDBWebAPI.ReportUtil;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

Console.OutputEncoding = Encoding.UTF8;
//appsettings.jsonのConnectionStrings:ReceiveUrlを設定する
IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
//builder にUseUrlsにappsettings.jsonのConnectionStrings:ReceiveUrlを設定する
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ← Microsoftログを止める（または制御可能にする）
builder.Logging.ClearProviders();
// ← Serilog を ASP.NET Core に統合
//builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
//    .ReadFrom.Configuration(context.Configuration)
//    .ReadFrom.Services(services)
//    .Enrich.FromLogContext()
//);

builder.WebHost.UseUrls(configuration["ConnectionStrings:ReceiveUrl"] ?? throw new NullReferenceException());

// Aspireのサービス規定値を追加
builder.AddServiceDefaults();
　
//ログ出力準備
//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .CreateLogger();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddBlazorDownloadFile(ServiceLifetime.Scoped);

builder.Services.AddControllersWithViews();

WebApplication app = builder.Build();
LogTo.Information("Start WebAPI");

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

CommonInfo.RootPath = app.Environment.ContentRootPath;

DataSource.setDataSource();

// wasm側でGetFromJsonAsyncした時「TypeError:Failed to fetch」が発生する対応
// 参考URL
// https://stackoverflow.com/questions/72359131/blazor-httprequestexceptiontypeerrorfailed-to-fetch
app.UseCors(_ => _
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin  
    .AllowCredentials());               // allow credentials 

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


//nishiyama Add
//APIエンドポイントを登録
app.MapPost("/api/label/excel", (CreateLabelRequest req) =>
{
    var (bytes, fileName, mime) = ReportFunc.物品管理ラベル発行(
req.DEVICE_ID, req.USER_ID, req.PalletNo, req.社員コード, req.管理責任者,
        req.部署, req.プロジェクト名, req.保管開始日, req.保管終了日, req.仕掛番号, req.内容, req.部, req.課);
    if (bytes is null || bytes.Length == 0) return Results.BadRequest("生成失敗");
    return Results.File(bytes, mime, fileName); // ← ブラウザはダウンロード扱い
    　　　　　　　　　　　　　　　　　　　　　　//メモリ上のbyte[]をファイルとして返却
});

app.MapPost("/api/label/excel2", (CreateLabelRequest2 req) =>
{
    var (bytes, fileName, mime) = ReportFunc.バーコードラベル発行(
req.DEVICE_ID, req.USER_ID, req.棚番);
    if (bytes is null || bytes.Length == 0) return Results.BadRequest("生成失敗");
    return Results.File(bytes, mime, fileName); // ← ブラウザはダウンロード扱い
                                                //メモリ上のbyte[]をファイルとして返却
});


app.Run();




// nishiyama
public sealed record CreateLabelRequest(
    string DEVICE_ID, string USER_ID, string PalletNo, string 社員コード, string 管理責任者,
    string 部署, string プロジェクト名, string 保管開始日, string 保管終了日, string 仕掛番号,string 内容, string 部, string 課);

public sealed record CreateLabelRequest2(
    string DEVICE_ID, string USER_ID, string 棚番);
