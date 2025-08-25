﻿namespace UserService.Domain.Models;

public sealed class CarePlanMeasurementTemplate : DomainEntity<Guid>
{
    public RecordType RecordType { get; private set; }       // Loại chỉ số: BloodGlucose, BloodPressure...
    public HealthCarePlanPeriodType? Period { get; private set; }               // Thời điểm đo: morning, before_sleep...
    public TimeOnly ScheduledAt { get; private set; }               // Thời điểm đo: morning, before_sleep...
    public HealthCarePlanSubTypeType? Subtype { get; private set; }             // Ngữ cảnh: fasting, pre-lunch, etc.
    public string? Reason { get; private set; }              // Lý do đo (AI giải thích)

    public Guid PatientProfileId { get; private set; }
    public PatientProfile PatientProfile { get; private set; }
    
    // DoctorProfile
    public Guid? DoctorProfileId { get; private set; }
    public DoctorProfile? DoctorProfile { get; private set; } = default!;
    private CarePlanMeasurementTemplate() { } // For EF Cores

    public CarePlanMeasurementTemplate(
        Guid id,
        Guid patientProfileId,
        RecordType recordType,
        HealthCarePlanPeriodType? period,
        TimeOnly scheduledAt,
        HealthCarePlanSubTypeType? subtype,
        string? reason,
        Guid? doctorProfileId)
    {
        Id = id;
        PatientProfileId = patientProfileId;
        RecordType = recordType;
        Period = period;
        ScheduledAt = scheduledAt;
        Subtype = subtype;
        Reason = reason;
        DoctorProfileId = doctorProfileId;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = DateTime.UtcNow;
        IsDeleted = false;
    }

    public static CarePlanMeasurementTemplate Create(
        Guid id,
        Guid patientProfileId,
        RecordType recordType,
        HealthCarePlanPeriodType? period,
        TimeOnly scheduledAt,
        HealthCarePlanSubTypeType? subtype,
        string? reason,
        Guid? doctorProfileId)
    {
        return new CarePlanMeasurementTemplate(
            id,
            patientProfileId,
            recordType,
            period,
            scheduledAt,
            subtype,
            reason,
            doctorProfileId
        );
    }
    
    public void Update(
        RecordType recordType,
        HealthCarePlanPeriodType? period,
        TimeOnly scheduledAt,
        HealthCarePlanSubTypeType? subtype)
    {
        RecordType = recordType;
        Period = period;
        ScheduledAt = scheduledAt;
        Subtype = subtype;
        ModifiedDate = DateTime.UtcNow;
        Reason = null;
    }
}