using UserService.Contract.Services.Patients.Responses;

namespace UserService.Contract.Services.Patients.Queries;

public record GetCarePlanMeasurementQuery
   (Guid UserId, Guid? DoctorId, DateTimeOffset? FromDate = null, DateTimeOffset? ToDate = null) : IQuery<Success<List<CarePlanInstanceResponse>>>;
