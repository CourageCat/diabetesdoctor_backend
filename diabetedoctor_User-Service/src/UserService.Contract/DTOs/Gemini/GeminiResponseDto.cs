namespace UserService.Contract.DTOs.Gemini;

public class GeminiResponseDto
{
    public CandidateDto[] Candidates { get; set; }
    public PromptFeedbackDto PromptFeedback { get; set; }
}