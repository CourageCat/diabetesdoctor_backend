using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Hospitals.Filteres;
using UserService.Contract.Services.Hospitals.Queries;
using UserService.Contract.Services.Hospitals.Responses;

namespace UserService.Application.UseCases.V1.Queries.Hospitals;

public sealed class GetAllHospitalsByAdminQueryHandler(ApplicationDbContext context) : IQueryHandler<GetAllHospitalsByAdminQuery, Success<OffsetPagedResult<HospitalResponse>>>
{
    public async Task<Result<Success<OffsetPagedResult<HospitalResponse>>>> Handle(GetAllHospitalsByAdminQuery query, CancellationToken cancellationToken)
    {
        var adminFound = await context.AdminProfiles.Where(admin => admin.UserId == query.AdminId && admin.IsDeleted == false).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (adminFound is null)
        {
            return FailureFromMessage(AdminErrors
                .AdminNotFound);
        }
        
        var hospitalQuery = context.HospitalProfiles
            .AsSplitQuery();
        hospitalQuery = ApplyFilter(hospitalQuery, query.Filters, adminFound.Id);
        
        var totalCount = await hospitalQuery.CountAsync(cancellationToken: cancellationToken);
        if (totalCount == 0)
        {
            var emptyResult = OffsetPagedResult<HospitalResponse>.CreateEmpty(query.Pagination.PageIndex, query.Pagination.PageSize);
            return Result.Success(new Success<OffsetPagedResult<HospitalResponse>>(
                HospitalMessages.GetAllHospitalsSuccessfully.GetMessage().Code,
                HospitalMessages.GetAllHospitalsSuccessfully.GetMessage().Message, 
                emptyResult));
        }
        
        hospitalQuery = ApplySorting(hospitalQuery, query.Filters);
        var hospitalQueryProjection = ApplyProjection(hospitalQuery);
        
        var result = await OffsetPagedResult<HospitalResponse>.CreateAsync(hospitalQueryProjection, query.Pagination.PageIndex, query.Pagination.PageSize, totalCount);
        return Result.Success(new Success<OffsetPagedResult<HospitalResponse>>(
            HospitalMessages.GetAllHospitalsSuccessfully.GetMessage().Code,
            HospitalMessages.GetAllHospitalsSuccessfully.GetMessage().Message,
            result));
    }
    
    private IQueryable<HospitalProfile> ApplyFilter(IQueryable<HospitalProfile> query, GetAllHospitalsByAdminFilter filter, Guid adminId)
    {
        // Always filter out deleted hospitals
        query = query.Where(x => 
            x.AdminProfileId == adminId
            && x.IsDeleted == false);

        // Search by name or introduction
        if (!string.IsNullOrEmpty(filter.Search))
        {
            query = query.Where(x =>
                x.Name.ToLower().Contains(filter.Search.ToLower()) 
                || x.Introduction.Contains(filter.Search));
        }
        
        return query;
    }
    
    private IQueryable<HospitalProfile> ApplySorting(IQueryable<HospitalProfile> query, GetAllHospitalsByAdminFilter filter)
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
    
    private IQueryable<HospitalResponse> ApplyProjection(IQueryable<HospitalProfile> query)
    {
        return query
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
    
    private static Result<Success<OffsetPagedResult<HospitalResponse>>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<OffsetPagedResult<HospitalResponse>>>(error);
    }
}