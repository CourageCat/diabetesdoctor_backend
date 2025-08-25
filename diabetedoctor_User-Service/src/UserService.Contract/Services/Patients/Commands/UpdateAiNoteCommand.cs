namespace UserService.Contract.Services.Patients.Commands;

public record UpdateAiNoteCommand : ICommand<Success>
{
    public Guid HealthRecordId { get; init; }
    public Guid UserId { get; init; }
}