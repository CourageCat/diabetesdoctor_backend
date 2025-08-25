using Microsoft.AspNetCore.Http;
using UserService.Contract.Services.Patients.Responses;

namespace UserService.Contract.Services.Patients.Commands;

public class UploadFilesCommand : ICommand<Success<IEnumerable<MediaResponse>>>
{
    public IFormFileCollection Images { get; init; } = default!;
    public string UploadedBy { get; init; } = default!;
}