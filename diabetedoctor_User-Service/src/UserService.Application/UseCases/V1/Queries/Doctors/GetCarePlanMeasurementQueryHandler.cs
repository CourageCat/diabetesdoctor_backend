using UserService.Contract.Common.DomainErrors;
using UserService.Contract.DTOs.Doctor;
using UserService.Contract.Services.Doctors.Queries;
using UserService.Contract.Services.Doctors.Responses;

namespace UserService.Application.UseCases.V1.Queries.Doctors;

public sealed class GetCarePlanMeasurementQueryHandler
    : IQueryHandler<GetCarePlanMeasurementQuery, Success<List<CarePlanInstanceResponse>>>
{
    private readonly ApplicationDbContext _context;

    public GetCarePlanMeasurementQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Success<List<CarePlanInstanceResponse>>>> Handle(
        GetCarePlanMeasurementQuery query,
        CancellationToken cancellationToken)
    {
        var patientId = await _context.PatientProfiles
            .Where(p => p.UserId == query.PatientId)
            .Select(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (patientId == Guid.Empty)
            return FailureFromMessage(PatientErrors.ProfileNotExist);
        
        var doctorId = await _context.DoctorProfiles
            .Where(p => p.UserId == query.DoctorId)
            .Select(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (doctorId == Guid.Empty)
            return FailureFromMessage(DoctorErrors.ProfileNotExist);
        
        var today = DateTimeHelper.ToLocalTimeNow(NationEnum.VietNam);
        var fromDate = query.FromDate.HasValue ? DateTime.SpecifyKind(query.FromDate.Value.Date, DateTimeKind.Utc) : today.Date;
        var toDate = query.ToDate.HasValue ? DateTime.SpecifyKind(query.ToDate.Value.Date, DateTimeKind.Utc) : today.AddDays(1).Date;
        var rawInstances = await _context.CarePlanMeasurementInstances
            .Include(x => x.DoctorProfile)
            .ThenInclude(x => x!.User)
            .Where(x =>
                x.PatientProfileId == patientId && 
                x.DoctorProfileId == doctorId &&
                x.ScheduledAt.AddHours(7).Date >= fromDate &&
                x.ScheduledAt.AddHours(7).Date <= toDate)
            .Select(x => new {
                x.Id,
                x.RecordType,
                x.Period,
                x.Subtype,
                x.Reason,
                x.ScheduledAt,
                x.MeasuredAt,
                x.IsCompleted,
                x.DoctorProfile
            })
            .OrderByDescending(x => x.ScheduledAt)
            .ToListAsync(cancellationToken);

        var instances = rawInstances
            .Select(x => new CarePlanInstanceResponse{
                Id = x.Id.ToString(),
                RecordType = x.RecordType.ToEnum<RecordType, RecordEnum>(),
                Period = x.Period.ToEnumNullable<HealthCarePlanPeriodType, HealthCarePlanPeriodEnum>(),
                Subtype = x.Subtype.ToEnumNullable<HealthCarePlanSubTypeType, HealthCarePlanSubTypeEnum>(),
                Reason = x.Reason,
                ScheduledAt = x.ScheduledAt,
                MeasuredAt = x.MeasuredAt,
                IsCompleted = x.IsCompleted,
                Doctor = x.DoctorProfile != null ? new DoctorDto()
                {
                    Id = x.DoctorProfile.Id.ToString(),
                    Name = x.DoctorProfile.User.DisplayName,
                    Avatar = x.DoctorProfile.User.Avatar.Url
                } : null
            })
            .ToList();

        return Result.Success(new Success<List<CarePlanInstanceResponse>>("", "", instances));
    }

    private static Result<Success<List<CarePlanInstanceResponse>>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<List<CarePlanInstanceResponse>>>(error);
    }
}