using System.Text;
using UserService.Contract.Services.Hospitals.Filteres;
using UserService.Contract.Services.Hospitals.Queries;
using UserService.Contract.Services.Hospitals.Responses;

namespace UserService.Application.UseCases.V1.Queries.Hospitals;

public sealed class GetAllHospitalsQueryHandler(ApplicationDbContext context) : IQueryHandler<GetAllHospitalsQuery, Success<CursorPagedResult<HospitalResponse>>>
{
    public async Task<Result<Success<CursorPagedResult<HospitalResponse>>>> Handle(GetAllHospitalsQuery query, CancellationToken cancellationToken)
    {
        var hospitalQuery = context.HospitalProfiles
            .AsSplitQuery();
        var cursorValues = new List<string>();
        if (query.Pagination.Cursor != null)
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(query.Pagination.Cursor));
            cursorValues.AddRange(decoded.Split('|').ToList());
        }

        hospitalQuery = ApplyFilter(hospitalQuery, query.Filters, cursorValues);
        hospitalQuery = ApplySorting(hospitalQuery, query.Filters);
        var hospitalFoundAfterPagination = await hospitalQuery.Take(query.Pagination.PageSize + 1).ToListAsync(cancellationToken);
        var hasNext = hospitalFoundAfterPagination.Count > query.Pagination.PageSize;
        var nextCursor = "";
        if (hasNext)
        {
            hospitalFoundAfterPagination.RemoveRange(query.Pagination.PageSize, hospitalFoundAfterPagination.Count - query.Pagination.PageSize);
            var lastHospital = hospitalFoundAfterPagination.ToList()[^1];
            nextCursor = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{GetSortCursorValue(lastHospital, query.Filters.SortBy)}|{lastHospital.Id}"));
        }
        var hospitalProjection = ApplyProjection(hospitalFoundAfterPagination).ToList();
        var result = CursorPagedResult<HospitalResponse>.Create(hospitalProjection, query.Pagination.PageSize, nextCursor, hasNext);
        return Result.Success(new Success<CursorPagedResult<HospitalResponse>>(
            HospitalMessages.GetAllHospitalsSuccessfully.GetMessage().Code,
            HospitalMessages.GetAllHospitalsSuccessfully.GetMessage().Message,
            result));
    }
    
        private IQueryable<HospitalProfile> ApplyFilter(IQueryable<HospitalProfile> query, GetAllHospitalsFilter filter, List<string> cursorValues)
    {
        // Always filter out deleted hospitals
        query = query.Where(x => x.IsDeleted == false);

        // Filter Cursor
        var isAsc = filter.SortDirection == SortDirectionEnum.Asc;
        if (cursorValues.Count != 0)
        {
            var isParseSecondCursorValueSuccess = Guid.TryParse(cursorValues[1], out Guid idInCursor);
            if (isParseSecondCursorValueSuccess)
            {
                if (filter.SortBy.ToLower().Equals("date"))
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
                else
                {
                    if (isAsc)
                    {
                        query = query.Where(x =>
                            x.Name.CompareTo(cursorValues[0]) > 0 ||
                            (x.Name.Equals(cursorValues[0]) && x.Id > idInCursor));
                    }
                    else
                    {
                        query = query.Where(x =>
                            x.Name.CompareTo(cursorValues[0]) < 0 ||
                            (x.Name.Equals(cursorValues[0]) && x.Id > idInCursor));
                    }
                }
            }
        }

        // Search by name or introduction
        if (!string.IsNullOrEmpty(filter.Search))
        {
            query = query.Where(x =>
                x.Name.ToLower().Contains(filter.Search.ToLower())
                || x.Introduction.Contains(filter.Search));
        }
        return query;
    }

    private IQueryable<HospitalProfile> ApplySorting(IQueryable<HospitalProfile> query, GetAllHospitalsFilter filter)
    {
        var isAsc = filter.SortDirection == SortDirectionEnum.Asc;
        query = filter.SortBy switch
        {
            "createdDate" => isAsc 
                ? query.OrderBy(x => x.CreatedDate).ThenByDescending(x => x.Id) 
                : query.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.Id),
            _ => isAsc
                ? query.OrderBy(x => x.Name).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
        };

        return query;
    }
    
    private IEnumerable<HospitalResponse> ApplyProjection(IEnumerable<HospitalProfile> hospitalProfiles)
    {
        return hospitalProfiles
            .Select(hospital => new HospitalResponse()
            {
                Id = hospital.Id.ToString(),
                Name = hospital.Name,
                Email = hospital.Email,
                PhoneNumber = hospital.PhoneNumber,
                Website =  hospital.Website,
                Address = hospital.Address,
                Thumbnail = hospital.Thumbnail.Url,
                CreatedDate = hospital.CreatedDate,
            });
    }
    
    private string GetSortCursorValue(HospitalProfile hospital, string sortBy)
    {
        var result = sortBy switch
        {
            "createdDate" => hospital.CreatedDate.ToString()!,
            _ => hospital.Name,
        };
        return result;
    }
    
    private static Result<Success<CursorPagedResult<HospitalResponse>>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<CursorPagedResult<HospitalResponse>>>(error);
    }
}