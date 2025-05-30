using CVBuilder.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly HttpClient _client;
    private readonly string _apiKey;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ChatController> logger)
    {
        _client = httpClientFactory.CreateClient();
        _apiKey = configuration["OpenAI_ApiKey"];
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] GptRequest gptRequest)
    {
        try
        {
            var prompt = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = gptRequest.Prompt },
                    new { role = "user", content = gptRequest.Question }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Content = JsonContent.Create(prompt)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("שגיאה מהשרת: {Error}", error);
                return StatusCode((int)response.StatusCode, error);
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var content = json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "שגיאה בבקשה ל-OpenAI");
            return StatusCode(500, "שגיאה בשרת.");
        }
    }
}
