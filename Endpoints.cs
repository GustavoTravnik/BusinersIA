using BusinersIA.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusinersIA;

public static class Endpoints
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        app.MapPost("/getResponse", ([FromBody] ResponseRequest request, [FromServices] OllamaProcessingService ollamaService) =>
        {
            var processId = ollamaService.StartProcessing(request.Input, request.ModelName);
            return Results.Created($"/getResponseResult/{processId}", new { processId = processId });
        })
        .WithName("GetResponse");

        app.MapGet("/getResponseResult/{id:guid}", (Guid id, [FromServices] OllamaProcessingService ollamaService) =>
        {
            var state = ollamaService.GetProcessState(id);
            if (state == null)
            {
                return Results.NotFound(new { message = "Process not found" });
            }

            return Results.Ok(new 
            { 
                id = state.Id, 
                status = state.Status.ToString(), 
                content = state.Content 
            });
        })
        .WithName("GetResponseResult");

        app.MapGet("/getAvailableModels", async ([FromServices] IHttpClientFactory httpClientFactory) =>
        {
            var client = httpClientFactory.CreateClient();
            try
            {
                var response = await client.GetAsync("http://localhost:11434/api/tags");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return Results.Content(content, "application/json");
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error connecting to Ollama API: {ex.Message}");
            }
        })
        .WithName("GetAvailableModels");

        app.MapPost("/enableModel", (string modelName) =>
        {
            if (string.IsNullOrWhiteSpace(modelName))
            {
                return Results.BadRequest(new { message = "Model name cannot be empty." });
            }

            try
            {
                var processInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "ollama",
                    Arguments = $"pull {modelName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                System.Diagnostics.Process.Start(processInfo);
                
                return Results.Ok(new { message = $"Started installing model '{modelName}' via ollama pull." });
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error executing ollama command: {ex.Message}");
            }
        })
        .WithName("EnableModel");
    }
}

public record ResponseRequest(string Input, string ModelName);
