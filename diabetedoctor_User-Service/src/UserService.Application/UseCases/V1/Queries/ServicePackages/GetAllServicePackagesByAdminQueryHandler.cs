using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.ServicePackages.Filters;
using UserService.Contract.Services.ServicePackages.Queries;
using UserService.Contract.Services.ServicePackages.Responses;

namespace UserService.Application.UseCases.V1.Queries.ServicePackages;

public sealed class GetAllServicePackagesByAdminQueryHandler(ApplicationDbContext context) :  IQueryHandler<GetAllServicePackagesByAdminQuery, Success<OffsetPagedResult<ServicePackageResponse>>>
{
    public async Task<Result<Success<OffsetPagedResult<ServicePackageResponse>>>> Handle(GetAllServicePackagesByAdminQuery query, CancellationToken cancellationToken)
    {
        var adminFound = await context.AdminProfiles.Where(admin => admin.UserId == query.AdminId && admin.IsDeleted == false).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (adminFound is null)
        {
            return FailureFromMessage(AdminErrors
                .AdminNotFound);
        }
        
        var servicePackageQuery = context.ServicePackages
            .AsSplitQuery();
        servicePackageQuery = ApplyFilter(servicePackageQuery, query.Filters, adminFound.Id);
        
        var totalCount = await servicePackageQuery.CountAsync(cancellationToken: cancellationToken);
        if (totalCount == 0)
        {
            var emptyResult = OffsetPagedResult<ServicePackageResponse>.CreateEmpty(query.Pagination.PageIndex, query.Pagination.PageSize);
            return Result.Success(new Success<OffsetPagedResult<ServicePackageResponse>>(
                ServicePackageMessages.GetAllServicePackagesSuccessfully.GetMessage().Code,
                ServicePackageMessages.GetAllServicePackagesSuccessfully.GetMessage().Message, 
                emptyResult));
        }
        
        servicePackageQuery = ApplySorting(servicePackageQuery, query.Filters);
        var servicePackageQueryProjection = ApplyProjection(servicePackageQuery);
        
        var result = await OffsetPagedResult<ServicePackageResponse>.CreateAsync(servicePackageQueryProjection, query.Pagination.PageIndex, query.Pagination.PageSize, totalCount);
        return Result.Success(new Success<OffsetPagedResult<ServicePackageResponse>>(
            ServicePackageMessages.GetAllServicePackagesSuccessfully.GetMessage().Code,
            ServicePackageMessages.GetAllServicePackagesSuccessfully.GetMessage().Message,
            result));
    }
    
    private IQueryable<ServicePackage> ApplyFilter(IQueryable<ServicePackage> query, GetAllServicePackagesByAdminFilter filter, Guid adminId)
    {
        // Always filter out deleted servicePackages
        query = query.Where(x => 
            x.AdminProfileId == adminId
            && x.IsDeleted == false);

        // Search by name or description
        if (!string.IsNullOrEmpty(filter.Search))
        {
            query = query.Where(x =>
                x.Name.ToLower().Contains(filter.Search.ToLower()) 
                || x.Description.Contains(filter.Search.ToLower()));
        }
        
        // Filter by IsActive
        if (filter.IsActive != null)
        {
            query = query.Where(x => x.IsActive == filter.IsActive);
        }
        
        return query;
    }
    
    private IQueryable<ServicePackage> ApplySorting(IQueryable<ServicePackage> query, GetAllServicePackagesByAdminFilter filter)
    {
        var isAsc = filter.SortDirection == SortDirectionEnum.Asc;
        query = filter.SortBy switch
        {
            "price" => isAsc
                ? query.OrderBy(x => x.Price).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.Price).ThenByDescending(x => x.Id),
            "sessions" => isAsc
                ? query.OrderBy(x => x.Sessions).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.Sessions).ThenByDescending(x => x.Id),
            "durationInMonths" => isAsc
                ? query.OrderBy(x => x.DurationInMonths).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.DurationInMonths).ThenByDescending(x => x.Id),
            "createdDate" => isAsc
                ? query.OrderBy(x => x.CreatedDate).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.CreatedDate).ThenByDescending(x => x.Id),
            _ => isAsc
                ? query.OrderBy(x => x.Name).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
        };

        return query;
    }
    
    private IQueryable<ServicePackageResponse> ApplyProjection(IQueryable<ServicePackage> query)
    {
        return query
            .Select(servicePackage => new ServicePackageResponse
            {
                Id = servicePackage.Id.ToString(),
                Name = servicePackage.Name,
                Price =  servicePackage.Price,
                Sessions = servicePackage.Sessions,
                DurationInMonths = servicePackage.DurationInMonths,
                IsActive = servicePackage.IsActive,
                CreatedDate = servicePackage.CreatedDate!.Value,
            });
    }
    
    private static Result<Success<OffsetPagedResult<ServicePackageResponse>>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<OffsetPagedResult<ServicePackageResponse>>>(error);
    }
}