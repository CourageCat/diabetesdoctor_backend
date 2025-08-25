using NotificationService.Contract.DTOs.ValueObjectDtos;

namespace NotificationService.Contract.Services.User;

public record UpdateUserCommand : ICommand
{
    public string UserId { get; init; } = null!;
    public FullNameDto? FullName { get; init; }
    public string? Avatar { get; init; }
};