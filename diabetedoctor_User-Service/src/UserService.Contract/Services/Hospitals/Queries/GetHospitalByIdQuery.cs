using UserService.Contract.Services.Hospitals.Responses;

namespace UserService.Contract.Services.Hospitals.Queries;

public record GetHospitalByIdQuery : IQuery<Success<HospitalResponse>>
{
    public Guid HospitalId { get; init; }
}