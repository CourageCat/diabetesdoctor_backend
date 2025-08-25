using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Patients.Queries;

namespace UserService.Application.UseCases.V1.Queries.Patients;

public sealed class GetHealthRecordValuesQueryHandler
    : IQueryHandler<GetHealthRecordValuesQuery, Success<GetHealthRecordResponse>>
{
    private readonly ApplicationDbContext _context;

    public GetHealthRecordValuesQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Success<GetHealthRecordResponse>>> Handle(
        GetHealthRecordValuesQuery query,
        CancellationToken cancellationToken)
    {
        var patientId = await _context.PatientProfiles
            .Where(p => p.UserId == query.UserId)
            .Select(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (patientId == Guid.Empty)
            return FailureFromMessage(PatientErrors.ProfileNotExist);

        List<RecordType>? recordTypes = null;
        if (query.RecordTypes is { Count: > 0 })
            recordTypes = query.RecordTypes.Select(r => r.ToEnum<RecordEnum, RecordType>()).ToList();
        
        var today = DateTimeHelper.ToLocalTimeNow(NationEnum.VietNam);
        var fromDate = query.FromDate.HasValue ? DateTime.SpecifyKind(query.FromDate.Value.Date, DateTimeKind.Utc) : today.Date;
        var toDate = query.ToDate.HasValue ? DateTime.SpecifyKind(query.ToDate.Value.Date, DateTimeKind.Utc) : today.AddDays(1).Date;
        IQueryable<HealthRecord> healthRecordsQuery;
        if (!query.IsBelongToDoctorTemplate)
        {
            healthRecordsQuery = _context.HealthRecords
                .Where(hr => hr.PatientProfileId == patientId
                             && hr.MeasuredAt.AddHours(7).Date >= fromDate
                             && hr.MeasuredAt.AddHours(7).Date <= toDate);
        }
        else
        {
            healthRecordsQuery = _context.HealthRecords
                .Where(hr => hr.PatientProfileId == patientId
                             && hr.CarePlanMeasurementInstance != null ? hr.CarePlanMeasurementInstance!.DoctorProfileId != null : true
                             && hr.MeasuredAt.AddHours(7).Date >= fromDate
                             && hr.MeasuredAt.AddHours(7).Date <= toDate); 
        }

        if (recordTypes is not null)
            healthRecordsQuery = healthRecordsQuery.Where(hr => recordTypes.Contains(hr.RecordType));
        
        if (query.Newest)
        {
            if (query.OnePerType)
            {
                healthRecordsQuery = healthRecordsQuery
                    .GroupBy(hr => hr.RecordType)
                    .Select(g => g.OrderByDescending(hr => hr.MeasuredAt).First());
            }
            else
            {
                healthRecordsQuery = healthRecordsQuery
                    .OrderByDescending(hr => hr.MeasuredAt);
            }
        }

        var healthRecords = await healthRecordsQuery.AsNoTracking().ToListAsync(cancellationToken);

        var dtos = healthRecords.Select(hr => new HealthRecordDto(
            hr.PatientProfileId,
            hr.RecordType.ToEnum<RecordType, RecordEnum>(),
            MapHealthRecordValueToDto(hr.RecordType, hr.RecordValue),
            hr.MeasuredAt,
            hr.PersonNote,
            hr.AssistantNote
        )).ToList();
        // Generate Response with null value for not found Record Type
        AddNotFoundRecordTypeInHealthRecords(dtos, query.RecordTypes);

        (double? min, double? max) min_max = (null, null);
        if (recordTypes?.Count == 1)
        {
            var recordType = recordTypes.First();

            var values = await healthRecordsQuery
                .Where(hr => hr.RecordType == recordType)
                .ToListAsync(cancellationToken);

            if (values.Any())
            {
                var extractedValues = values.Select(hr => recordType switch
                {
                    RecordType.Weight => ((WeightValue)hr.RecordValue).Value,
                    RecordType.Height => ((HeightValue)hr.RecordValue).Value,
                    RecordType.BloodGlucose => ((BloodGlucoseValue)hr.RecordValue).Value,
                    RecordType.BloodPressure => ((BloodPressureValue)hr.RecordValue).Systolic,
                    RecordType.HbA1c => ((HbA1cValue)hr.RecordValue).Value,
                    _ => 0
                }).Where(v => v > 0);

                if (extractedValues.Any())
                {
                    min_max = (extractedValues.Min(), extractedValues.Max());
                }
            }
        }
        var minmax = recordTypes?.Count == 1 ? new MinMax(min_max.max ?? 0, min_max.min ?? 0) : null;
        var response = new GetHealthRecordResponse(HealthRecords: dtos, MinMax: minmax);
        return Result.Success(new Success<GetHealthRecordResponse>("", "", response));
    }

    private void AddNotFoundRecordTypeInHealthRecords(List<HealthRecordDto> healthRecords, List<RecordEnum> recordTypes)
    {
        var listRecordTypesNotFoundFromHealthRecords = recordTypes
            .Where(recordType => !healthRecords.Select(hr => hr.RecordType).ToList().Contains(recordType)).ToList();
        listRecordTypesNotFoundFromHealthRecords.ForEach(recordType =>
        {
            healthRecords.Add(new HealthRecordDto(null, recordType));
        });
    }

    private static HealthRecordValueDto? MapHealthRecordValueToDto(RecordType recordType, HealthRecordValue value)
    {
        return recordType switch
        {
            RecordType.Weight when value is WeightValue weight =>
                new WeightValueDto("weight", weight.Value, weight.Unit),

            RecordType.Height when value is HeightValue height =>
                new HeightValueDto("height", height.Value, height.Unit),

            RecordType.BloodGlucose when value is BloodGlucoseValue glucose =>
                new BloodGlucoseValueDto("blood_glucose", glucose.Value, glucose.Unit,
                    glucose.Level.ToEnum<BloodGlucoseLevelType, BloodGlucoseLevelEnum>(),
                    glucose.MeasureTimeType.ToEnum<BloodGlucoseMeasureTimeType, BloodGlucoseMeasureTimeEnum>()),

            RecordType.BloodPressure when value is BloodPressureValue bp =>
                new BloodPressureValueDto("blood_pressure", bp.Systolic, bp.Diastolic, bp.Unit),

            RecordType.HbA1c when value is HbA1cValue hba1c =>
                new HbA1cValueDto("hba1c", hba1c.Value, hba1c.Unit),

            _ => null
        };
    }

    private static double GetValueByType(HealthRecord hr, RecordType type) => type switch
    {
        RecordType.Weight => ((WeightValue)hr.RecordValue).Value,
        RecordType.Height => ((HeightValue)hr.RecordValue).Value,
        RecordType.BloodGlucose => ((BloodGlucoseValue)hr.RecordValue).Value,
        RecordType.BloodPressure => ((BloodPressureValue)hr.RecordValue).Systolic,
        RecordType.HbA1c => ((HbA1cValue)hr.RecordValue).Value,
        _ => 0
    };

    private static Result<Success<GetHealthRecordResponse>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<GetHealthRecordResponse>> (error);
    }
}