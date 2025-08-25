using UserService.Contract.Common.DomainErrors;
using UserService.Contract.DTOs.Doctor;
using UserService.Contract.Services.Patients.Queries;

namespace UserService.Application.UseCases.V1.Queries.Patients;

public sealed class GetAllDoctorCreatedTemplateQueryHandler(ApplicationDbContext context) : IQueryHandler<GetAllDoctorCreatedTemplateQuery, Success<IEnumerable<DoctorDto>>>
{
    public async Task<Result<Success<IEnumerable<DoctorDto>>>> Handle(GetAllDoctorCreatedTemplateQuery request, CancellationToken cancellationToken)
    {
        var patientFound = await context.PatientProfiles.FirstOrDefaultAsync(patient => patient.UserId == request.PatientId, cancellationToken);
        if (patientFound is null)
        {
            return Result.Failure<Success<IEnumerable<DoctorDto>>>(PatientErrors.ProfileNotExist);
        }
        var doctorIdsCreatedTemplateFound = await context.CarePlanMeasurements
            .Where(template => template.PatientProfileId == patientFound.Id)
            .GroupBy(template => template.DoctorProfileId)
            .Select(g => g.Key)
            .ToListAsync(cancellationToken);
        var doctorsCreatedTemplate = await context.DoctorProfiles.Include(doctor => doctor.User).Where(doctor =>  doctorIdsCreatedTemplateFound.Contains(doctor.Id)).ToListAsync(cancellationToken);
        var result = MapToListDoctorDtos(doctorsCreatedTemplate);
        return Result.Success(new Success<IEnumerable<DoctorDto>>(PatientMessages.GetAllDoctorsCreatedTemplateSuccessfully.GetMessage().Code, PatientMessages.GetAllDoctorsCreatedTemplateSuccessfully.GetMessage().Message, result));
    }

    private IEnumerable<DoctorDto> MapToListDoctorDtos(IEnumerable<DoctorProfile> doctorProfiles)
    {
        return doctorProfiles.ToList().Select(doctor =>
            new DoctorDto()
            {
                Id = doctor.UserId.ToString(),
                Name = doctor.User.DisplayName,
                Avatar = doctor.User.Avatar.Url,
            }
        );
    }
}