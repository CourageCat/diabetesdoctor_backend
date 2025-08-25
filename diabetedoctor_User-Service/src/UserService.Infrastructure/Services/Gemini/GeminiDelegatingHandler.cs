using Microsoft.Extensions.Options;

namespace UserService.Infrastructure.Services.Gemini;

internal sealed class GeminiDelegatingHandler(IOptions<GeminiSettings> geminiSettings) 
    : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("x-goog-api-key", $"{geminiSettings.Value.ApiKey}");

        return base.SendAsync(request, cancellationToken);
    }
}