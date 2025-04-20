using GetParamsFile;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


var getParams = new GetParams();

app.MapGet("/{*urlParam}", (string urlParam = "") => {
  return getParams.Get(urlParam);
});

app.Run();
