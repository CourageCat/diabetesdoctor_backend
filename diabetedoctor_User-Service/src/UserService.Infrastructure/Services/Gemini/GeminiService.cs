using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UserService.Contract.DTOs.Gemini;
using UserService.Contract.Infrastructure;

namespace UserService.Infrastructure.Services.Gemini;

public class GeminiService :  IAiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeminiService> _logger;
    private readonly JsonSerializerSettings _serializerSettings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        }
    };
    private readonly GeminiSettings _geminiSettings;

    public GeminiService(HttpClient httpClient, ILogger<GeminiService> logger, IOptions<GeminiSettings> geminiConfigs)
    {
        _httpClient = httpClient;
        _logger = logger;
        _geminiSettings = geminiConfigs.Value;
    }

    public async Task<string?> GenerateContentAsync(string prompt, CancellationToken cancellationToken)
    {
        var requestBody = GeminiRequestFactory.CreateRequest(prompt);
        var content = new StringContent(JsonConvert.SerializeObject(requestBody, Formatting.None, _serializerSettings), Encoding.UTF8, "application/json");
        var url = $"{_geminiSettings.Url}?key={_geminiSettings.ApiKey}";
        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        var geminiResponse = JsonConvert.DeserializeObject<GeminiResponseDto>(responseBody);

        var geminiResponseText = geminiResponse?.Candidates[0].Content.Parts[0].Text;

        return geminiResponseText;
    }
}