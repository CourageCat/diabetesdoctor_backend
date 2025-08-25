using System.Text;
using UserService.Contract.DTOs.ServicePackage;
using UserService.Contract.Services.ServicePackages.Filters;
using UserService.Contract.Services.ServicePackages.Queries;
using UserService.Contract.Services.ServicePackages.Responses;

namespace UserService.Application.UseCases.V1.Queries.ServicePackages;

public sealed class GetAllServicePackagesPurchasedQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetAllServicePackagesPurchasedQuery, Success<CursorPagedResult<ServicePackagePurchasedResponse>>>
{
    public async Task<Result<Success<CursorPagedResult<ServicePackagePurchasedResponse>>>> Handle(
        GetAllServicePackagesPurchasedQuery query, CancellationToken cancellationToken)
    {
        var userPackagesQuery = context.UserPackages
            .Include(up => up.PaymentHistory)
            .AsSplitQuery();
        var cursorValues = new List<string>();
        if (query.Pagination.Cursor != null)
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(query.Pagination.Cursor));
            cursorValues.AddRange(decoded.Split('|').ToList());
        }

        userPackagesQuery = ApplyFilter(userPackagesQuery, query.Filters, cursorValues, query.UserId);
        userPackagesQuery = ApplySorting(userPackagesQuery, query.Filters);
        var userPackagesFoundAfterPagination =
            await userPackagesQuery.Take(query.Pagination.PageSize + 1).ToListAsync(cancellationToken);
        var hasNext = userPackagesFoundAfterPagination.Count > query.Pagination.PageSize;
        var nextCursor = "";
        if (hasNext)
        {
            userPackagesFoundAfterPagination.RemoveRange(query.Pagination.PageSize,
                userPackagesFoundAfterPagination.Count - query.Pagination.PageSize);
            var lastServicePackage = userPackagesFoundAfterPagination.ToList()[^1];
            nextCursor = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                $"{GetSortCursorValue(lastServicePackage, query.Filters.SortBy)}|{lastServicePackage.Id}"));
        }

        var userPackagesProjection = ApplyProjection(userPackagesFoundAfterPagination).ToList();
        var result = CursorPagedResult<ServicePackagePurchasedResponse>.Create(userPackagesProjection,
            query.Pagination.PageSize, nextCursor, hasNext);
        return Result.Success(new Success<CursorPagedResult<ServicePackagePurchasedResponse>>(
            ServicePackageMessages.GetAllServicePackagesSuccessfully.GetMessage().Code,
            ServicePackageMessages.GetAllServicePackagesSuccessfully.GetMessage().Message,
            result));
    }

    private IQueryable<UserPackage> ApplyFilter(IQueryable<UserPackage> query, GetAllServicePackagesPurchasedFilter filter,
        List<string> cursorValues, Guid userId)
    {
        // Always filter out deleted userPackages
        query = query.Where(x =>
            x.IsDeleted == false
            && x.UserId == userId);

        // Filter Cursor
        var isAsc = filter.SortDirection == SortDirectionEnum.Asc;
        if (cursorValues.Count != 0)
        {
            var isParseSecondCursorValueSuccess = Guid.TryParse(cursorValues[1], out Guid idInCursor);
            if (isParseSecondCursorValueSuccess)
            {
                if (filter.SortBy.ToLower().Equals("priceAtPurchased"))
                {
                    var isParseFirstCursorValueSuccess =
                        double.TryParse(cursorValues[0], out double priceAtPurchasedInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.PaymentHistory.Amount > priceAtPurchasedInCursor ||
                                (x.PaymentHistory.Amount == priceAtPurchasedInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.PaymentHistory.Amount < priceAtPurchasedInCursor ||
                                (x.PaymentHistory.Amount == priceAtPurchasedInCursor && x.Id < idInCursor));
                        }
                    }
                }

                if (filter.SortBy.ToLower().Equals("totalSessions"))
                {
                    var isParseFirstCursorValueSuccess =
                        int.TryParse(cursorValues[0], out int totalSessionsInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.TotalSessions > totalSessionsInCursor ||
                                (x.TotalSessions == totalSessionsInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.TotalSessions < totalSessionsInCursor ||
                                (x.TotalSessions == totalSessionsInCursor && x.Id < idInCursor));
                        }
                    }
                }
                if (filter.SortBy.ToLower().Equals("remainingSessions"))
                {
                    var isParseFirstCursorValueSuccess =
                        int.TryParse(cursorValues[0], out int remainingSessionsInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.RemainingSessions > remainingSessionsInCursor ||
                                (x.RemainingSessions == remainingSessionsInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.RemainingSessions < remainingSessionsInCursor ||
                                (x.RemainingSessions == remainingSessionsInCursor && x.Id < idInCursor));
                        }
                    }
                }
                if (filter.SortBy.ToLower().Equals("expireDate"))
                {
                    var isParseFirstCursorValueSuccess =
                        DateTime.TryParse(cursorValues[0], out DateTime expireDateInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.ExpireDate > expireDateInCursor ||
                                (x.ExpireDate == expireDateInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.ExpireDate < expireDateInCursor ||
                                (x.ExpireDate == expireDateInCursor && x.Id < idInCursor));
                        }
                    }
                }
                if (filter.SortBy.ToLower().Equals("purchasedDate"))
                {
                    var isParseFirstCursorValueSuccess =
                        DateTime.TryParse(cursorValues[0], out DateTime purchasedDateInCursor);
                    if (isParseFirstCursorValueSuccess)
                    {
                        if (isAsc)
                        {
                            query = query.Where(x =>
                                x.CreatedDate > purchasedDateInCursor ||
                                (x.CreatedDate == purchasedDateInCursor && x.Id > idInCursor));
                        }
                        else
                        {
                            query = query.Where(x =>
                                x.CreatedDate < purchasedDateInCursor ||
                                (x.CreatedDate == purchasedDateInCursor && x.Id < idInCursor));
                        }
                    }
                }
                else
                {
                    if (isAsc)
                    {
                        query = query.Where(x =>
                            x.PackageName.CompareTo(cursorValues[0]) > 0 ||
                            (x.PackageName.Equals(cursorValues[0]) && x.Id > idInCursor));
                    }
                    else
                    {
                        query = query.Where(x =>
                            x.PackageName.CompareTo(cursorValues[0]) < 0 ||
                            (x.PackageName.Equals(cursorValues[0]) && x.Id > idInCursor));
                    }
                }
            }
        }
        

        // Search by name or introduction
        if (!string.IsNullOrEmpty(filter.Search))
        {
            query = query.Where(x =>
                x.PackageName.ToLower().Contains(filter.Search.ToLower()));
        }
        
        // Filter by IsExpired
        if (filter.IsExpired != null)
        {
            query = query.Where(x => x.IsExpired == filter.IsExpired);
        }
        
        // Filter by IsExistedSessions
        if (filter.IsExistedSessions != null)
        {
            if (filter.IsExistedSessions.Value)
            {
                query = query.Where(x => x.RemainingSessions > 0);
            }
            else
            {
                query = query.Where(x => x.RemainingSessions <= 0);
            }
        }

        return query;
    }

    private IQueryable<UserPackage> ApplySorting(IQueryable<UserPackage> query,
        GetAllServicePackagesPurchasedFilter filter)
    {
        var isAsc = filter.SortDirection == SortDirectionEnum.Asc;
        query = filter.SortBy switch
        {
            "priceAtPurchased" => isAsc
                ? query.OrderBy(x => x.PaymentHistory.Amount).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.PaymentHistory.Amount).ThenByDescending(x => x.Id),
            "totalSessions" => isAsc
                ? query.OrderBy(x => x.TotalSessions).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.TotalSessions).ThenByDescending(x => x.Id),
            "remainingSessions" => isAsc
                ? query.OrderBy(x => x.RemainingSessions).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.RemainingSessions).ThenByDescending(x => x.Id),
            "expireDate" => isAsc
                ? query.OrderBy(x => x.ExpireDate).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.ExpireDate).ThenByDescending(x => x.Id),
            "purchasedDate" => isAsc
                ? query.OrderBy(x => x.CreatedDate).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.Id),
            _ => isAsc
                ? query.OrderBy(x => x.PackageName).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.PackageName).ThenByDescending(x => x.Id),
        };

        return query;
    }

    private IEnumerable<ServicePackagePurchasedResponse> ApplyProjection(IEnumerable<UserPackage> userPackages)
    {
        return userPackages
            .Select(userPackage => new ServicePackagePurchasedResponse
                {
                    Id = userPackage.Id.ToString(),
                    PackageName = userPackage.PackageName,
                    PriceAtPurchased = userPackage.PaymentHistory.Amount,
                    TotalSessions = userPackage.TotalSessions,
                    RemainingSessions = userPackage.RemainingSessions,
                    ExpireDate = userPackage.ExpireDate,
                    IsExpired = userPackage.IsExpired,
                    PurchasedDate = userPackage.CreatedDate!.Value,
                }
            );
    }

    private string GetSortCursorValue(UserPackage userPackage, string sortBy)
    {
        var result = sortBy switch
        {
            "priceAtPurchased" => userPackage.PaymentHistory.Amount.ToString(),
            "totalSessions" => userPackage.TotalSessions.ToString()!,
            "remainingSessions" => userPackage.RemainingSessions.ToString(),
            "expireDate" => userPackage.ExpireDate.ToString(),
            "purchasedDate" => userPackage.CreatedDate.ToString(),
            _ => userPackage.PackageName,
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