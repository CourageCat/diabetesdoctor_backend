using UserService.Contract.DTOs.Gemini;

namespace UserService.Infrastructure.Services.Gemini;

internal sealed class GeminiRequestFactory
{
    public static GeminiRequestDto CreateRequest(string prompt)
    {
        return new GeminiRequestDto()
        {
            Contents = new GeminiContentDto[]
            {
                new GeminiContentDto()
                {
                    Role = "user",
                    Parts = new GeminiPartDto[]
                    {
                        new GeminiPartDto()
                        {
                            Text = prompt
                        }
                    }
                }
            },
            GenerationConfig = new GenerationConfigDto()
            {
                Temperature = 0,
                TopK = 1,
                TopP = 1,
                MaxOutputTokens = 2048,
                StopSequences = new List<object>()
            },
            SafetySettings = new SafetySettingsDto[]
            {
                new SafetySettingsDto
                {
                    Category = "HARM_CATEGORY_HARASSMENT",
                    Threshold = "BLOCK_ONLY_HIGH"
                },
                new SafetySettingsDto
                {
                    Category = "HARM_CATEGORY_HATE_SPEECH",
                    Threshold = "BLOCK_ONLY_HIGH"
                },
                new SafetySettingsDto
                {
                    Category = "HARM_CATEGORY_SEXUALLY_EXPLICIT",
                    Threshold = "BLOCK_ONLY_HIGH"
                },
                new SafetySettingsDto
                {
                    Category = "HARM_CATEGORY_DANGEROUS_CONTENT",
                    Threshold = "BLOCK_ONLY_HIGH"
                }
            }
        };
    }
}