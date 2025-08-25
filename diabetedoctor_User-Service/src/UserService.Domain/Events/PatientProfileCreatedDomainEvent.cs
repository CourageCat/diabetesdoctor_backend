using UserService.Contract.Services.Patients.Commands;

namespace UserService.Domain.Events;

public record PatientProfileCreatedDomainEvent(Guid Id, Guid PatientId, CreatePatientProfileCommand PatientProfile) : IDomainEvent
{
    public static PatientProfileCreatedDomainEvent Create(CreatePatientProfileCommand patientProfile, Guid patientId, Guid? id = null)
    {
        var actualId = (id is null || id == Guid.Empty) ? Guid.NewGuid() : id.Value;
        return new PatientProfileCreatedDomainEvent(actualId, patientId, patientProfile);
    }
}