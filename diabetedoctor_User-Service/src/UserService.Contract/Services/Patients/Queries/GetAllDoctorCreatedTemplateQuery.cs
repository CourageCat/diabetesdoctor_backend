using UserService.Contract.DTOs.Doctor;

namespace UserService.Contract.Services.Patients.Queries;

public record GetAllDoctorCreatedTemplateQuery : IQuery<Success<IEnumerable<DoctorDto>>>
{
    public Guid PatientId { get; init; }
}