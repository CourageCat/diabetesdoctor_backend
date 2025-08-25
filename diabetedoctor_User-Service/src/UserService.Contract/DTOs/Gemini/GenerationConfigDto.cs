namespace UserService.Contract.DTOs.Gemini;

public class GenerationConfigDto
{
    public int Temperature { get; set; }
    public int TopK { get; set; }
    public int TopP { get; set; }
    public int MaxOutputTokens { get; set; }
    public List<object> StopSequences { get; set; }
}