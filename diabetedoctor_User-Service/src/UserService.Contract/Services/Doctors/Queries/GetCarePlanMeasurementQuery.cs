using UserService.Contract.Services.Doctors.Responses;

namespace UserService.Contract.Services.Doctors.Queries;

public record GetCarePlanMeasurementQuery
   (Guid PatientId, Guid DoctorId, DateTimeOffset? FromDate = null, DateTimeOffset? ToDate = null) : IQuery<Success<List<CarePlanInstanceResponse>>>;
