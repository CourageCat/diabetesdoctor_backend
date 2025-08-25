using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Hospitals.Filteres;
using UserService.Contract.Services.Hospitals.Queries;
using UserService.Contract.Services.Hospitals.Responses;

namespace UserService.Application.UseCases.V1.Queries.Hospitals;

public sealed class GetAllHospitalStaffsByAdminQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetAllHospitalStaffsByAdminQuery, Success<OffsetPagedResult<HospitalStaffResponse>>>
{
    public async Task<Result<Success<OffsetPagedResult<HospitalStaffResponse>>>> Handle(
        GetAllHospitalStaffsByAdminQuery query, CancellationToken cancellationToken)
    {
        var hospitalAdminFound = await context.HospitalAdmins
            .Where(hospitalAdmin => hospitalAdmin.UserId == query.HospitalAdminId && hospitalAdmin.IsDeleted == false)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (hospitalAdminFound is null)
        {
            return FailureFromMessage(AdminErrors
                .AdminNotFound);
        }

        var hospitalStaffQuery = context.HospitalStaffs
            .Include(hospitalStaff => hospitalStaff.User)
            .AsSplitQuery();
        hospitalStaffQuery = ApplyFilter(hospitalStaffQuery, query.Filters, hospitalAdminFound.Id);

        var totalCount = await hospitalStaffQuery.CountAsync(cancellationToken: cancellationToken);
        if (totalCount == 0)
        {
            var emptyResult =
                OffsetPagedResult<HospitalStaffResponse>.CreateEmpty(query.Pagination.PageIndex, query.Pagination.PageSize);
            return Result.Success(new Success<OffsetPagedResult<HospitalStaffResponse>>(
                HospitalStaffMessages.GetAllHospitalStaffsSuccessfully.GetMessage().Code,
                HospitalStaffMessages.GetAllHospitalStaffsSuccessfully.GetMessage().Message,
                emptyResult));
        }

        hospitalStaffQuery = ApplySorting(hospitalStaffQuery, query.Filters);
        var hospitalStaffQueryProjection = ApplyProjection(hospitalStaffQuery);

        var result = await OffsetPagedResult<HospitalStaffResponse>.CreateAsync(hospitalStaffQueryProjection,
            query.Pagination.PageIndex, query.Pagination.PageSize, totalCount);
        return Result.Success(new Success<OffsetPagedResult<HospitalStaffResponse>>(
            HospitalMessages.GetAllHospitalsSuccessfully.GetMessage().Code,
            HospitalMessages.GetAllHospitalsSuccessfully.GetMessage().Message,
            result));
    }

        private IQueryable<HospitalStaff> ApplyFilter(IQueryable<HospitalStaff> query, GetAllHospitalStaffsByAdminFilter filter, Guid hospitalAdminId)
    {
        // Always filter out deleted doctors
        query = query.Where(x =>
            x.HospitalAdminId == hospitalAdminId &&
            x.User.IsDeleted == false
            && x.IsDeleted == false);
        
        // Search by name or introduction
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
        
    private IQueryable<HospitalStaff> ApplySorting(IQueryable<HospitalStaff> query,
        GetAllHospitalStaffsByAdminFilter filter)
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

    private IQueryable<HospitalStaffResponse> ApplyProjection(IQueryable<HospitalStaff> query)
    {
        return query
            .Select(hospitalStaff => new HospitalStaffResponse()
            {
                Id = hospitalStaff.UserId.ToString(),
                Email = hospitalStaff.User.Email!,
                Avatar = hospitalStaff.User.Avatar.Url,
                Name = hospitalStaff.User.DisplayName,
                DateOfBirth = hospitalStaff.User.DateOfBirth,
                Gender = (GenderEnum)hospitalStaff.User.Gender,
                CreatedDate = hospitalStaff.CreatedDate,
            });
    }

    private static Result<Success<OffsetPagedResult<HospitalStaffResponse>>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<OffsetPagedResult<HospitalStaffResponse>>>(error);
    }
}