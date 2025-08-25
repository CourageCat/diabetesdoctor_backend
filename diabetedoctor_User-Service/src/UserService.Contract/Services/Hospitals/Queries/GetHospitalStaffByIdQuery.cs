using UserService.Contract.Services.Hospitals.Responses;

namespace UserService.Contract.Services.Hospitals.Queries;

public record GetHospitalStaffByIdQuery : IQuery<Success<HospitalStaffResponse>>
{
    public Guid HospitalStaffId { get; init; }
}