using RepoDb;
using Serilog;
using ZennohWebAPI.Common;

//appsettings.jsonのConnectionStrings:ReceiveUrlを設定する
IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
//builder にUseUrlsにappsettings.jsonのConnectionStrings:ReceiveUrlを設定する
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls(configuration["ConnectionStrings:ReceiveUrl"] ?? throw new NullReferenceException());

//ログ出力準備
//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .CreateLogger();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//初期化１回だけ
_ = GlobalConfiguration
    .Setup()
    .UseSqlServer();

builder.Services.AddControllersWithViews();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

CommonInfo.RootPath = app.Environment.ContentRootPath;

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

app.Run();
