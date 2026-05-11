using BusinersIA;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

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

// app.UseHttpsRedirection(); // Comentado para permitir requisições HTTP locais pelo cURL sem redirecionamento 307

app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.MapApiEndpoints();

app.Run();
