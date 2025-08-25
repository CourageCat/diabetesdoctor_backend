namespace MediaService.Contract.Infrastructure.Services;

public interface IUserContext
{
    string? UserId { get; }
    string? Role { get; }
}
