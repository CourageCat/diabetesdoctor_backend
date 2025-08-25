namespace UserService.Contract.Infrastructure;

public interface IUserContext
{
    string? UserId { get; }
    string? Role { get; }
}
