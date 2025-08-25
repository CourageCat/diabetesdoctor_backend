using System.Text;
using UserService.Contract.DTOs.ServicePackage;
using UserService.Contract.Services.ServicePackages.Filters;
using UserService.Contract.Services.ServicePackages.Queries;
using UserService.Contract.Services.ServicePackages.Responses;

namespace UserService.Application.UseCases.V1.Queries.ServicePackages;

public sealed class GetAllServicePackagesQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetAllServicePackagesQuery, Success<CursorPagedResult<ServicePackageResponse>>>
{
    public async Task<Result<Success<CursorPagedResult<ServicePackageResponse>>>> Handle(
        GetAllServicePackagesQuery query, CancellationToken cancellationToken)
    {
        var servicePackagesQuery = context.ServicePackages
            .AsSplitQuery();
        var cursorValues = new List<string>();
        if (query.Pagination.Cursor != null)
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(query.Pagination.Cursor));
            cursorValues.AddRange(decoded.Split('|').ToList());
        }

        servicePackagesQuery = ApplyFilter(servicePackagesQuery, query.Filters, cursorValues);
        servicePackagesQuery = ApplySorting(servicePackagesQuery, query.Filters);
        var servicePackagesFoundAfterPagination =
            await servicePackagesQuery.Take(query.Pagination.PageSize + 1).ToListAsync(cancellationToken);
        var hasNext = servicePackagesFoundAfterPagination.Count > query.Pagination.PageSize;
        var nextCursor = "";
        if (hasNext)
        {
            servicePackagesFoundAfterPagination.RemoveRange(query.Pagination.PageSize,
                servicePackagesFoundAfterPagination.Count - query.Pagination.PageSize);
            var lastServicePackage = servicePackagesFoundAfterPagination.ToList()[^1];
            nextCursor = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                $"{GetSortCursorValue(lastServicePackage, query.Filters.SortBy)}|{lastServicePackage.Id}"));
        }

        var servicePackagesProjection = ApplyProjection(servicePackagesFoundAfterPagination).ToList();
        var result = CursorPagedResult<ServicePackageResponse>.Create(servicePackagesProjection,
            query.Pagination.PageSize, nextCursor, hasNext);
        return Result.Success(new Success<CursorPagedResult<ServicePackageResponse>>(
            ServicePackageMessages.GetAllServicePackagesSuccessfully.GetMessage().Code,
            ServicePackageMessages.GetAllServicePackagesSuccessfully.GetMessage().Message,
            result));
    }

    private IQueryable<ServicePackage> ApplyFilter(IQueryable<ServicePackage> query, GetAllServicePackagesFilter filter,
        List<string> cursorValues)
    {
        // Always filter out deleted servicePackagess
        query = query.Where(x =>
            x.IsDeleted == false);

        // Filter Cursor
        var isAsc = filter.SortDirection == SortDirectionEnum.Asc;
        if (cursorValues.Count != 0)
        {
            var isParseSecondCursorValueSuccess = Guid.TryParse(cursorValues[1], out Guid idInCursor);
            if (isParseSecondCursorValueSuccess)
            {
                if (filter.SortBy.ToLower().Equals("price"))
                {
                    var isParseFirstCursorValueSuccess =
                        double.TryParse(cursorValues[0], out double priceInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.Price > priceInCursor ||
                                (x.Price == priceInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.Price < priceInCursor ||
                                (x.Price == priceInCursor && x.Id < idInCursor));
                        }
                    }
                }

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
                if (filter.SortBy.ToLower().Equals("sessions"))
                {
                    var isParseFirstCursorValueSuccess =
                        int.TryParse(cursorValues[0], out int sessionsInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.Sessions > sessionsInCursor ||
                                (x.Sessions == sessionsInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.Sessions < sessionsInCursor ||
                                (x.Sessions == sessionsInCursor && x.Id < idInCursor));
                        }
                    }
                }
                if (filter.SortBy.ToLower().Equals("durationInMonths"))
                {
                    var isParseFirstCursorValueSuccess =
                        int.TryParse(cursorValues[0], out int durationInMonthsInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.DurationInMonths > durationInMonthsInCursor ||
                                (x.DurationInMonths == durationInMonthsInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.DurationInMonths < durationInMonthsInCursor ||
                                (x.DurationInMonths == durationInMonthsInCursor && x.Id < idInCursor));
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
                x.Name.ToLower().Contains(filter.Search.ToLower()) ||
                x.Description.ToLower().Contains(filter.Search.ToLower()));
        }

        return query;
    }

    private IQueryable<ServicePackage> ApplySorting(IQueryable<ServicePackage> query,
        GetAllServicePackagesFilter filter)
    {
        var isAsc = filter.SortDirection == SortDirectionEnum.Asc;
        query = filter.SortBy switch
        {
            "price" => isAsc
                ? query.OrderBy(x => x.Price).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.Price).ThenByDescending(x => x.Id),
            "createdDate" => isAsc
                ? query.OrderBy(x => x.CreatedDate).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.Id),
            "sessions" => isAsc
                ? query.OrderBy(x => x.Sessions).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.Sessions).ThenByDescending(x => x.Id),
            "durationInMonths" => isAsc
                ? query.OrderBy(x => x.DurationInMonths).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.DurationInMonths).ThenByDescending(x => x.Id),
            _ => isAsc
                ? query.OrderBy(x => x.Name).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
        };

        return query;
    }

    private IEnumerable<ServicePackageResponse> ApplyProjection(IEnumerable<ServicePackage> servicePackages)
    {
        return servicePackages
            .Select(servicePackage => new ServicePackageResponse
                {
                    Id = servicePackage.Id.ToString(),
                    Name = servicePackage.Name,
                    Price = servicePackage.Price,
                    Sessions = servicePackage.Sessions,
                    DurationInMonths = servicePackage.DurationInMonths,
                    IsActive = servicePackage.IsActive,
                    CreatedDate = servicePackage.CreatedDate!.Value,
                }
            );
    }

    private string GetSortCursorValue(ServicePackage servicePackage, string sortBy)
    {
        var result = sortBy switch
        {
            "price" => servicePackage.Price.ToString(),
            "createdDate" => servicePackage.CreatedDate.ToString()!,
            "sessions" => servicePackage.Sessions.ToString(),
            "durationInMonths" => servicePackage.DurationInMonths.ToString(),
            _ => servicePackage.Name,
        };
        return result;
    }

    private static PackageFeatureValueDto? MapPackageFeatureValueToDto(PackageFeatureTypeType featureType,
        PackageFeatureValue value)
    {
        return featureType switch
        {
            PackageFeatureTypeType.MaxConsultation when value is MaxConsultationValue maxConsultation =>
                new MaxConsultationDto("max_consultaion", maxConsultation.Value),

            PackageFeatureTypeType.AdditionalNotes when value is AdditionalNotesValue additionalNotes =>
                new AdditionalNotesDto("additional_notes", additionalNotes.Value),

            _ => null
        };
    }
}