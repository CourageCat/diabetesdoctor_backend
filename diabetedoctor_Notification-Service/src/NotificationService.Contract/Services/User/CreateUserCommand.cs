using NotificationService.Contract.DTOs.ValueObjectDtos;

namespace NotificationService.Contract.Services.User;

public record CreateUserCommand : ICommand
{
    public string UserId { get; init; } = null!;
    public FullNameDto FullName { get; init; } = null!;
    public string Avatar { get; init; } = null!;
};