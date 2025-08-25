using ChatService.Contract.Enums;

namespace ChatService.Contract.Services.User.Filters;

public record GetAvailableUsersForConversationFilters
{
    public RoleEnum Role { get; init; }
    public string? Search { get; init; } = null;
}