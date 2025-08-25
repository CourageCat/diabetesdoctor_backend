namespace UserService.Contract.Services.Patients.Queries;

public record GetConsultationSessionsQuery : IQuery<Success<int>>
{
    public Guid UserId { get; init; }
}