using UserService.Contract.Common.Pagination;
using UserService.Contract.Services.Moderators.Filters;
using UserService.Contract.Services.Moderators.Responses;

namespace UserService.Contract.Services.Moderators.Queries;

public record GetAllModeratorsQuery : IQuery<Success<CursorPagedResult<ModeratorResponse>>>
{
    public CursorPaginationRequest Pagination { get; init; } = null!;
    public GetAllModeratorsFilter Filters { get; init; } = null!;
}