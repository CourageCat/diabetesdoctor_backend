using Microsoft.AspNetCore.Http;

namespace UserService.Contract.Services.Patients.Commands;

public record ChangeAvatarCommand : ICommand<Success>
{
    public Guid UserId { get; init; }
    public string Role { get; init; } = string.Empty;
    public IFormFile Avatar { get; init; } = null!;
}