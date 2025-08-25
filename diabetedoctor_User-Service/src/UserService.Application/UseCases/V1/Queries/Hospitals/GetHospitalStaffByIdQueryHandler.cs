using UserService.Contract.Common.DomainErrors;
using UserService.Contract.DTOs.Doctor;
using UserService.Contract.Services.Hospitals.Queries;
using UserService.Contract.Services.Hospitals.Responses;

namespace UserService.Application.UseCases.V1.Queries.Hospitals;

public sealed class GetHospitalStaffByIdQueryHandler(ApplicationDbContext context) : IQueryHandler<GetHospitalStaffByIdQuery, Success<HospitalStaffResponse>>
{
    public async Task<Result<Success<HospitalStaffResponse>>> Handle(GetHospitalStaffByIdQuery query, CancellationToken cancellationToken)
    {
        var hospitalStaffFound = await context.HospitalStaffs.Include(hospitalStaff => hospitalStaff.User)
            .Include(hospitalStaff => hospitalStaff.HospitalProfile)
            .FirstOrDefaultAsync(hospitalStaff => hospitalStaff.UserId == query.HospitalStaffId, cancellationToken: cancellationToken);
        if (hospitalStaffFound is null)
        {
            return FailureFromMessage(HospitalStaffErrors.HospitalStaffNotFound);
        }
        var result = new HospitalStaffResponse()
        {
            Id = hospitalStaffFound.UserId.ToString(),
            Email = hospitalStaffFound.User.Email!,
            Avatar = hospitalStaffFound.User.Avatar.Url,
            Name = hospitalStaffFound.User.DisplayName,
            DateOfBirth = hospitalStaffFound.User.DateOfBirth,
            Gender = (GenderEnum)hospitalStaffFound.User.Gender,
            CreatedDate = hospitalStaffFound.CreatedDate,
            Hospital = new HospitalDto
            {
                Id = hospitalStaffFound.HospitalProfile.Id.ToString(),
                PhoneNumber = hospitalStaffFound.HospitalProfile.PhoneNumber!,
                Name = hospitalStaffFound.HospitalProfile.Name,
                Thumbnail = hospitalStaffFound.HospitalProfile.Thumbnail.Url,
            }
        };
        
        return Result.Success(new Success<HospitalStaffResponse>(
            HospitalStaffMessages.GetHospitalStaffByIdSuccessfully.GetMessage().Code,
            HospitalStaffMessages.GetHospitalStaffByIdSuccessfully.GetMessage().Message,
            result));
    }
    
    private static Result<Success<HospitalStaffResponse>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<HospitalStaffResponse>>(error);
    }
}