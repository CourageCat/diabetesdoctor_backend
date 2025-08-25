namespace UserService.Contract.DTOs.Gemini;

public class GeminiContentDto
{
    public string Role { get; set; }
    public GeminiPartDto[] Parts { get; set; }
}