namespace UserService.Contract.Services.Users;

public record CreateUserCommand
    (Guid UserId) : ICommand<Success>;
