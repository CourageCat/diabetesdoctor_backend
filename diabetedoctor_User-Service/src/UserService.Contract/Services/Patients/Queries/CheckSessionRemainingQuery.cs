using UserService.Contract.Services.Patients.Responses;

namespace UserService.Contract.Services.Patients.Queries;

public record CheckSessionRemainingQuery : IQuery<Success<CheckSessionRemainingResponse>>
{
    public Guid UserId { get; init; }
}