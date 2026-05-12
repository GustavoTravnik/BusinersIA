using BusinersIA;
using BusinersIA.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<OllamaProcessingService>();

// Habilitar CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Verifica e instala o Ollama antes de iniciar os outros middlewares
SetupIA.CheckAndInstallOllamaAsync().GetAwaiter().GetResult();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Middleware para verificar o Ollama em todos os requests
app.Use(async (context, next) =>
{
    if (!SetupIA.IsOllamaInstalled())
    {
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await context.Response.WriteAsJsonAsync(new { erro = "O serviço está indisponível por falta do gerenciador de LLM (Ollama)." });
        return;
    }
    await next();
});

// app.UseHttpsRedirection(); // Comentado para permitir requisições HTTP locais pelo cURL sem redirecionamento 307

app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.MapApiEndpoints();

app.Run();
