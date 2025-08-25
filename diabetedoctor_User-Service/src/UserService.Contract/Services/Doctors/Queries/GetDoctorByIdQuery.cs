using UserService.Contract.Services.Doctors.Responses;

namespace UserService.Contract.Services.Doctors.Queries;

public record GetDoctorByIdQuery : IQuery<Success<DoctorResponse>>
{
    public Guid DoctorId { get; init; }
}