using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Logs estruturados em JSON. Regra do projeto: nunca logar conteúdo do CSV, só ids, contagens e durações.
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole(o =>
{
    o.IncludeScopes = true;
    o.UseUtcTimestamp = true;
    o.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
});

// Lotes de 100 mil a 500 mil linhas passam do limite padrão de 30 MB do Kestrel.
var limiteUploadBytes = builder.Configuration.GetValue("Upload:TamanhoMaximoMb", 512L) * 1024 * 1024;
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = limiteUploadBytes);
builder.Services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = limiteUploadBytes);

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins(builder.Configuration.GetSection("Cors:Origens").Get<string[]>() ?? [])
    .AllowAnyHeader()
    .AllowAnyMethod()));
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors();

app.MapOpenApi();
app.MapHealthChecks("/health");

app.Run();
