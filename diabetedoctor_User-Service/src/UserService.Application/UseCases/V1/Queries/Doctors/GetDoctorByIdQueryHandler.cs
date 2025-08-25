using UserService.Contract.Common.DomainErrors;
using UserService.Contract.DTOs.Doctor;
using UserService.Contract.Enums.Doctor;
using UserService.Contract.Services.Doctors.Queries;
using UserService.Contract.Services.Doctors.Responses;

namespace UserService.Application.UseCases.V1.Queries.Doctors;

public sealed class GetDoctorByIdQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetDoctorByIdQuery, Success<DoctorResponse>>
{
    public async Task<Result<Success<DoctorResponse>>> Handle(GetDoctorByIdQuery query, CancellationToken cancellationToken)
    {
        var doctorFound = await context.DoctorProfiles.Include(doctor => doctor.User)
            .Include(doctorProfile => doctorProfile.HospitalProfile)
            .FirstOrDefaultAsync(doctor => doctor.UserId == query.DoctorId, cancellationToken: cancellationToken);
        if (doctorFound is null)
        {
            return FailureFromMessage(DoctorErrors.DoctorNotFound);
        }

        var result = new DoctorResponse
        {
            Id = doctorFound.UserId.ToString(),
            PhoneNumber = doctorFound.User.PhoneNumber!,
            Avatar = doctorFound.User.Avatar.Url,
            Name = doctorFound.User.DisplayName,
            DateOfBirth = doctorFound.User.DateOfBirth,
            Gender = doctorFound.User.Gender.ToEnum<GenderType, GenderEnum>(),
            NumberOfExperiences = doctorFound.NumberOfExperiences,
            Position = doctorFound.Position.ToEnum<DoctorPositionType, DoctorPositionEnum>(),
            Introduction = doctorFound.Introduction,
            CreatedDate = doctorFound.CreatedDate,
            Hospital = new HospitalDto
            {
                Id = doctorFound.HospitalProfile.Id.ToString(),
                PhoneNumber = doctorFound.HospitalProfile.PhoneNumber!,
                Name = doctorFound.HospitalProfile.Name,
                Thumbnail = doctorFound.HospitalProfile.Thumbnail.Url,
            }
        };
        
        return Result.Success(new Success<DoctorResponse>(
            DoctorMessages.GetDoctorByIdSuccessfully.GetMessage().Code,
            DoctorMessages.GetDoctorByIdSuccessfully.GetMessage().Message,
            result));
    }
    
    private static Result<Success<DoctorResponse>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<DoctorResponse>>(error);
    }
}