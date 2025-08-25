namespace UserService.Contract.DTOs;

public record DiagnosisInfoDto
{
    public int? Year { get; init; } // Năm
    
    public DiagnosisRecencyEnum DiagnosisRecency { get; init; } 
}