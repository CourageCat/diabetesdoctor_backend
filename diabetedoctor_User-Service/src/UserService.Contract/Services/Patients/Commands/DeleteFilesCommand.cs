namespace UserService.Contract.Services.Patients.Commands;

public class DeleteFilesCommand : ICommand<Success>
{
    public List<string> ImageIds { get; init; } = default!;
}