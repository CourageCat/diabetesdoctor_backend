using UserService.Contract.Services.Patients.Responses;

namespace UserService.Contract.Services.Patients.Queries;

public record GetPatientProfileQuery
    (Guid UserId) : IQuery<Success<PatientProfileResponse>>;