namespace UserService.Contract.DTOs.Gemini;

public class GeminiRequestDto
{
    public GeminiContentDto[] Contents { get; set; }
    public GenerationConfigDto GenerationConfig { get; set; }
    public SafetySettingsDto[] SafetySettings { get; set; }
}