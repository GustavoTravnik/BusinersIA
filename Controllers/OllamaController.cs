using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinersIA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OllamaController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public OllamaController()
        {
            _httpClient = new HttpClient();
        }

        [HttpGet("prompt")]
        public async Task<IActionResult> GetPrompt([FromQuery] string prompt)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                return BadRequest("Prompt cannot be empty.");
            }

            var ollamaRequest = new
            {
                model = "gemma:2b", // ou o modelo que você estiver usando
                prompt = prompt,
                stream = false
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(ollamaRequest), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(responseContent);
                    var responseText = doc.RootElement.GetProperty("response").GetString();
                    return Ok(responseText);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Ollama API error: {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error connecting to Ollama: {ex.Message}");
            }
        }
    }
}