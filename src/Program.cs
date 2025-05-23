using GetParamsFile;
using GetLogsFile;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


var getParams = new GetParams();

var getLogs = new GetLogs();

app.MapGet("/{*urlParam}", (string urlParam = "") => {
  return getParams.Get(urlParam);
});

app.MapGet("/logs", (HttpContext context) => {
  context.Response.Headers.ContentType = "application/json";
  return getLogs.OnGetLogs();
});

app.Run();
