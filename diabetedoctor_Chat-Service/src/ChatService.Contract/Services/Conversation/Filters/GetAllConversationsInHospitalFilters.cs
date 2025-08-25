using ChatService.Contract.Enums;

namespace ChatService.Contract.Services.Conversation.Filters;

public record GetAllConversationsInHospitalFilters
{
    public string? Search { get; init; }
    public string SortBy { get; init; } = string.Empty;
    public SortDirectionEnum Direction { get; init; } = SortDirectionEnum.Desc;
}