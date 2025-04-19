using GetParamsFile;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


var getParams = new GetParams();

app.MapGet("/", (string param) => {
    getParams.Get(param);
});

app.Run();
