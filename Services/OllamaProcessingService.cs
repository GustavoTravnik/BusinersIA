using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace BusinersIA.Services;

public enum ProcessStatus
{
    Processing,
    Completed,
    Error
}

public class ProcessState
{
    public Guid Id { get; set; }
    public ProcessStatus Status { get; set; }
    public string? Content { get; set; }
}

public class OllamaProcessingService
{
    private readonly ConcurrentDictionary<Guid, ProcessState> _processes = new();
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OllamaProcessingService> _logger;

    public OllamaProcessingService(IHttpClientFactory httpClientFactory, ILogger<OllamaProcessingService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public Guid StartProcessing(string input, string modelName)
    {
        var id = Guid.NewGuid();
        _processes[id] = new ProcessState { Id = id, Status = ProcessStatus.Processing };

        // Run in background without awaiting
        _ = Task.Run(() => ProcessAsync(id, input, modelName));

        return id;
    }

    public ProcessState? GetProcessState(Guid id)
    {
        _processes.TryGetValue(id, out var state);
        return state;
    }

    private async Task ProcessAsync(Guid id, string input, string modelName)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(5); // Ollama can take a while to generate a response
            
            var requestBody = new
            {
                model = modelName,
                prompt = input,
                stream = true
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:11434/api/generate")
            {
                Content = jsonContent
            };

            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            var sb = new StringBuilder();

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var jsonDocument = JsonDocument.Parse(line);
                if (jsonDocument.RootElement.TryGetProperty("response", out var responseProperty))
                {
                    sb.Append(responseProperty.GetString());
                    if (_processes.TryGetValue(id, out var stateToUpdate))
                    {
                        stateToUpdate.Content = sb.ToString();
                    }
                }
            }

            if (_processes.TryGetValue(id, out var state))
            {
                state.Status = ProcessStatus.Completed;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Ollama request for ID {Id}", id);
            if (_processes.TryGetValue(id, out var state))
            {
                state.Status = ProcessStatus.Error;
                state.Content = $"Error: {ex.Message}";
            }
        }
    }
}
