using System.Text;

Console.OutputEncoding = Encoding.UTF8;
IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.ExpressionDBWebAPI>("WebAPI");
builder.AddProject<Projects.ExpressionDBBlazorWasmApp>("WASMapp");
//builder.AddProject<Projects.ExpressionDBCycleProcessApp>("CycleProcess");
//builder.AddProject<Projects.ExpressionDBCycleProcessWebApp>("CycleProcessWeb");
builder.Build().Run();
