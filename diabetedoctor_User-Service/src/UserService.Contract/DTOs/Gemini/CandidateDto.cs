namespace UserService.Contract.DTOs.Gemini;

public class CandidateDto
{
    public ContentDto Content { get; set; }
    public string FinishReason { get; set; }
    public int Index { get; set; }
    public SafetyRatingDto[] SafetyRatings { get; set; }
}