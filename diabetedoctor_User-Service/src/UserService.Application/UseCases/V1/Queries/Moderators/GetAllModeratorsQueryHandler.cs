using System.Text;
using UserService.Contract.Services.Moderators.Filters;
using UserService.Contract.Services.Moderators.Queries;
using UserService.Contract.Services.Moderators.Responses;

namespace UserService.Application.UseCases.V1.Queries.Moderators;

public sealed class GetAllModeratorsQueryHandler(ApplicationDbContext context) : IQueryHandler<GetAllModeratorsQuery, Success<CursorPagedResult<ModeratorResponse>>>
{
    public async Task<Result<Success<CursorPagedResult<ModeratorResponse>>>> Handle(GetAllModeratorsQuery query, CancellationToken cancellationToken)
    {
        var moderatorQuery = context.ModeratorProfiles
            .Include(moderator => moderator.User)
            .AsSplitQuery();
        var cursorValues = new List<string>();
        if (query.Pagination.Cursor != null)
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(query.Pagination.Cursor));
            cursorValues.AddRange(decoded.Split('|').ToList());
        }

        moderatorQuery = ApplyFilter(moderatorQuery, query.Filters, cursorValues);
        moderatorQuery = ApplySorting(moderatorQuery, query.Filters);
        var moderatorFoundAfterPagination = await moderatorQuery.Take(query.Pagination.PageSize + 1).ToListAsync(cancellationToken);
        var hasNext = moderatorFoundAfterPagination.Count > query.Pagination.PageSize;
        var nextCursor = "";
        if (hasNext)
        {
            moderatorFoundAfterPagination.RemoveRange(query.Pagination.PageSize, moderatorFoundAfterPagination.Count - query.Pagination.PageSize);
            var lastDoctor = moderatorFoundAfterPagination.ToList()[^1];
            nextCursor = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{GetSortCursorValue(lastDoctor, query.Filters.SortBy)}|{lastDoctor.Id}"));
        }
        var moderatorProjection = ApplyProjection(moderatorFoundAfterPagination).ToList();
        var result = CursorPagedResult<ModeratorResponse>.Create(moderatorProjection, query.Pagination.PageSize, nextCursor, hasNext);
        return Result.Success(new Success<CursorPagedResult<ModeratorResponse>>(
            ModeratorMessages.GetAllModeratorsSuccessfully.GetMessage().Code,
            ModeratorMessages.GetAllModeratorsSuccessfully.GetMessage().Message,
            result));
    }

    private IQueryable<ModeratorProfile> ApplyFilter(IQueryable<ModeratorProfile> query, GetAllModeratorsFilter filter, List<string> cursorValues)
    {
        // Always filter out deleted moderators
        query = query.Where(x =>
            x.User.IsDeleted == false
            && x.IsDeleted == false);

        // Filter Cursor
        var isAsc = filter.SortDirection == SortDirectionEnum.Asc;
        if (cursorValues.Count != 0)
        {
            var isParseSecondCursorValueSuccess = Guid.TryParse(cursorValues[1], out Guid idInCursor);
            if (isParseSecondCursorValueSuccess)
            {
                if (filter.SortBy.ToLower().Equals("createdDate"))
                {
                    var isParseFirstCursorValueSuccess =
                        DateTime.TryParse(cursorValues[0], out DateTime createdDateInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.CreatedDate > createdDateInCursor ||
                                (x.CreatedDate == createdDateInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.CreatedDate < createdDateInCursor ||
                                (x.CreatedDate == createdDateInCursor && x.Id < idInCursor));
                        }
                    }
                }
                else if (filter.SortBy.ToLower().Equals("dateOfBirth"))
                {
                    var isParseFirstCursorValueSuccess =
                        DateTime.TryParse(cursorValues[0], out DateTime dateOfBirthInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.User.DateOfBirth > dateOfBirthInCursor ||
                                (x.User.DateOfBirth == dateOfBirthInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.User.DateOfBirth < dateOfBirthInCursor ||
                                (x.User.DateOfBirth == dateOfBirthInCursor && x.Id < idInCursor));
                        }
                    }
                }
                else if (filter.SortBy.ToLower().Equals("gender"))
                {
                    var isParseFirstCursorValueSuccess =
                        Enum.TryParse(cursorValues[0], out GenderType genderInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.User.Gender > genderInCursor ||
                                (x.User.Gender == genderInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.User.Gender < genderInCursor ||
                                (x.User.Gender == genderInCursor && x.Id < idInCursor));
                        }
                    }
                }
                else
                {
                    if (isAsc)
                    {
                        query = query.Where(x =>
                            x.User.FullName.LastName.CompareTo(cursorValues[0]) > 0 ||
                            (x.User.FullName.LastName.Equals(cursorValues[0]) && x.Id > idInCursor));
                    }
                    else
                    {
                        query = query.Where(x =>
                            x.User.FullName.LastName.CompareTo(cursorValues[0]) < 0 ||
                            (x.User.FullName.LastName.Equals(cursorValues[0]) && x.Id > idInCursor));
                    }
                }
            }
        }

        // Search by name
        if (!string.IsNullOrEmpty(filter.Search))
        {
            query = query.Where(x =>
                x.User.DisplayName.ToLower().Contains(filter.Search.ToLower()));
        }

        // Filter by Gender
        if (filter.Gender != null)
        {
            var genderFilter = filter.Gender.Value.ToEnum<GenderEnum, GenderType>();
            query = query.Where(x => x.User.Gender == genderFilter);
        }
        
        return query;
    }

    private IQueryable<ModeratorProfile> ApplySorting(IQueryable<ModeratorProfile> query, GetAllModeratorsFilter filter)
    {
        var isAsc = filter.SortDirection == SortDirectionEnum.Asc;
        query = filter.SortBy switch
        {
            "createdDate" => isAsc
                ? query.OrderBy(x => x.CreatedDate).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.Id),
            "dateOfBirth" => isAsc 
                ? query.OrderBy(x => x.User.DateOfBirth).ThenByDescending(x => x.Id) 
                : query.OrderByDescending(x => x.User.DateOfBirth).ThenByDescending(x => x.Id),
            "gender" => isAsc 
                ? query.OrderBy(x => x.User.Gender).ThenByDescending(x => x.Id) 
                : query.OrderByDescending(x => x.User.Gender).ThenByDescending(x => x.Id),
            _ => isAsc
                ? query.OrderBy(x => x.User.FullName.LastName).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.User.FullName.LastName).ThenByDescending(x => x.Id),
        };

        return query;
    }

    private IEnumerable<ModeratorResponse> ApplyProjection(IEnumerable<ModeratorProfile> moderatorProfiles)
    {
        return moderatorProfiles
            .Select(moderator => new ModeratorResponse
            {
                Id = moderator.UserId.ToString(),
                Email = moderator.User.Email!,
                Avatar = moderator.User.Avatar.Url,
                Name = moderator.User.DisplayName,
                DateOfBirth = moderator.User.DateOfBirth,
                Gender = (GenderEnum)moderator.User.Gender,
                CreatedDate = moderator.CreatedDate,
            });
    }
    
    private string GetSortCursorValue(ModeratorProfile moderator, string sortBy)
    {
        var result = sortBy switch
        {
            "createdDate" => moderator.CreatedDate.ToString()!,
            "dateOfBirth" => moderator.User.DateOfBirth.ToString(),
            "gender" => moderator.User.Gender.ToString(),
            _ => moderator.User.FullName.LastName,
        };
        return result;
    }
    
    private static Result<Success<CursorPagedResult<ModeratorResponse>>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<CursorPagedResult<ModeratorResponse>>>(error);
    }
}