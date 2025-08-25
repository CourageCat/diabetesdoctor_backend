using ChatService.Contract.Common.Pagination;
using ChatService.Contract.DTOs.UserDtos;
using ChatService.Contract.Enums;
using ChatService.Contract.Services.User.Filters;

namespace ChatService.Contract.Services.User.Queries;

public record GetAvailableUsersForConversationQuery : IQuery<Response<PagedResult<UserResponseDto>>>
{
    public string UserId { get; init; } = null!;
    public ObjectId ConversationId { get; init; }
    public OffsetPaginationRequest Pagination { get; init; } = null!;
    public GetAvailableUsersForConversationFilters Filters { get; init; } = null!;
}