using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Enums.Doctor;
using UserService.Contract.Services.Doctors.Responses;
using UserService.Contract.Services.Hospitals.Filteres;
using UserService.Contract.Services.Hospitals.Queries;

namespace UserService.Application.UseCases.V1.Queries.Hospitals;

public sealed class GetAllDoctorsByStaffQueryHandler(ApplicationDbContext context) : IQueryHandler<GetAllDoctorsByStaffQuery, Success<OffsetPagedResult<DoctorResponse>>>
{
    public async Task<Result<Success<OffsetPagedResult<DoctorResponse>>>> Handle(GetAllDoctorsByStaffQuery query, CancellationToken cancellationToken)
    {
        var staffFound = await context.HospitalStaffs.Where(staff => staff.UserId == query.HospitalStaffId && staff.IsDeleted == false).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (staffFound is null)
        {
            return FailureFromMessage(HospitalStaffErrors
                .HospitalStaffNotFound);
        }
        
        var doctorQuery = context.DoctorProfiles
            .Include(doctor => doctor.User)
            .AsSplitQuery();
        doctorQuery = ApplyFilter(doctorQuery, query.Filters, staffFound.Id);
        
        var totalCount = await doctorQuery.CountAsync(cancellationToken: cancellationToken);
        if (totalCount == 0)
        {
            var emptyResult = OffsetPagedResult<DoctorResponse>.CreateEmpty(query.Pagination.PageIndex, query.Pagination.PageSize);
            return Result.Success(new Success<OffsetPagedResult<DoctorResponse>>(
                DoctorMessages.GetAllDoctorsSuccessfully.GetMessage().Code,
                DoctorMessages.GetAllDoctorsSuccessfully.GetMessage().Message, 
                emptyResult));
        }
        
        doctorQuery = ApplySorting(doctorQuery, query.Filters);
        var doctorQueryProjection = ApplyProjection(doctorQuery);
        
        var result = await OffsetPagedResult<DoctorResponse>.CreateAsync(doctorQueryProjection, query.Pagination.PageIndex, query.Pagination.PageSize, totalCount);
        return Result.Success(new Success<OffsetPagedResult<DoctorResponse>>(
            DoctorMessages.GetAllDoctorsSuccessfully.GetMessage().Code,
            DoctorMessages.GetAllDoctorsSuccessfully.GetMessage().Message,
            result));
    }
    
    private IQueryable<DoctorProfile> ApplyFilter(IQueryable<DoctorProfile> query, GetAllDoctorsByStaffFilter filter, Guid hospitalStaffId)
    {
        //Always filter out deleted doctors
        query = query.Where(x => 
            x.HospitalStaffId == hospitalStaffId
            && x.User.IsDeleted == false 
            && x.IsDeleted == false);
        query = query.Where(x => 
            x.User.IsDeleted == false 
            && x.IsDeleted == false);

        // Search by name or introduction
        if (!string.IsNullOrEmpty(filter.Search))
        {
            query = query.Where(x =>
                x.User.DisplayName.ToLower().Contains(filter.Search.ToLower()) 
                || x.Introduction.Contains(filter.Search));
        }
        
        // Filter by Gender
        if (filter.Gender != null)
        {
            var genderFilter = filter.Gender.Value.ToEnum<GenderEnum, GenderType>();
            query = query.Where(x => x.User.Gender == genderFilter);
        }
        
        // Filter by Position
        if (filter.Position != null)
        {
            var position = filter.Position.Value.ToEnum<DoctorPositionEnum, DoctorPositionType>();
            query = query.Where(x => x.Position == position);
        }
        
        return query;
    }
    
    private IQueryable<DoctorProfile> ApplySorting(IQueryable<DoctorProfile> query, GetAllDoctorsByStaffFilter filter)
    {
        var isAsc = filter.SortDirection == SortDirectionEnum.Asc;
        query = filter.SortBy switch
        {
            "experiences" => isAsc 
                ? query.OrderBy(x => x.NumberOfExperiences).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.NumberOfExperiences).ThenByDescending(x => x.Id),
            "position" => isAsc 
                ? query.OrderBy(x => x.Position).ThenByDescending(x => x.Id)
                : query.OrderByDescending(x => x.Position).ThenByDescending(x => x.Id),
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
    
    private IQueryable<DoctorResponse> ApplyProjection(IQueryable<DoctorProfile> query)
    {
        return query
            .Select(doctor => new DoctorResponse
            {
                Id = doctor.UserId.ToString(),
                PhoneNumber = doctor.User.PhoneNumber!,
                Avatar = doctor.User.Avatar.Url,
                Name = doctor.User.DisplayName,
                DateOfBirth = doctor.User.DateOfBirth,
                Gender = (GenderEnum)doctor.User.Gender,
                NumberOfExperiences = doctor.NumberOfExperiences,
                Position = (DoctorPositionEnum)doctor.Position,
                CreatedDate = doctor.CreatedDate
            });
    }
    
    private static Result<Success<OffsetPagedResult<DoctorResponse>>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<OffsetPagedResult<DoctorResponse>>>(error);
    }
}