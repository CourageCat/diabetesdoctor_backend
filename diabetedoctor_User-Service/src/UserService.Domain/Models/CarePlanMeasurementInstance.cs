﻿namespace UserService.Domain.Models;

public sealed class CarePlanMeasurementInstance : DomainEntity<Guid>
{
    // Ngày giờ dự kiến đo
    public DateTime ScheduledAt { get; private set; }

    // Trạng thái đo
    public bool IsCompleted { get; private set; }

    // Thời gian đã đo
    public DateTime? MeasuredAt { get; private set; }
    // Lịch đo đã thông báo với Bệnh nhân
    public bool IsNotified  { get; private set; }

    // PatientProfile
    public Guid PatientProfileId { get; private set; }
    public PatientProfile PatientProfile { get; private set; } = default!;

    // HealthRecord (nếu đã hoàn tất cập nhật)
    public Guid? HealthRecordId { get; private set; }
    public HealthRecord? HealthRecord { get; private set; }
    // DoctorProfile
    public Guid? DoctorProfileId { get; private set; }
    public DoctorProfile? DoctorProfile { get; private set; } = default!;
    // Snapshot lại các trường quan trọng của Template gốc
    public RecordType RecordType { get; private set; }
    public HealthCarePlanPeriodType? Period { get; private set; }
    public HealthCarePlanSubTypeType? Subtype { get; private set; }
    public string? Reason { get; private set; }

    private CarePlanMeasurementInstance()
    {
    }

    public static CarePlanMeasurementInstance CreateFromTemplate(
        Guid id,
        Guid patientProfileId,
        CarePlanMeasurementTemplate template,
        DateTime scheduledAt)
    {
        return new CarePlanMeasurementInstance
        {
            Id = id,
            PatientProfileId = patientProfileId,
            RecordType = template.RecordType,
            Period = template.Period,
            Subtype = template.Subtype,
            Reason = template.Reason,
            IsNotified = false,
            DoctorProfileId = template.DoctorProfileId,
            ScheduledAt = scheduledAt,
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false
        };
    }
    
    public static CarePlanMeasurementInstance Create(
        Guid id,
        Guid patientProfileId,
        Guid? doctorProfileId, 
        RecordType recordType,
        HealthCarePlanSubTypeType? subtype,
        DateTime scheduledAt)
    {
        return new CarePlanMeasurementInstance
        {
            Id = id,
            PatientProfileId = patientProfileId,
            DoctorProfileId = doctorProfileId,
            RecordType = recordType,
            Subtype = subtype,
            IsNotified = false,
            ScheduledAt = scheduledAt,
            IsCompleted = false,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    public void Update(
        RecordType recordType, HealthCarePlanSubTypeType? subtype,
        DateTime scheduledAt)
    {
        RecordType = recordType;
        Period = null;
        Subtype = subtype;
        ScheduledAt = scheduledAt;
        Reason = null;
        ModifiedDate = DateTime.UtcNow;
    }

    public void MarkCompleted(DateTime measuredAt, Guid healthRecordId)
    {
        IsCompleted = true;
        MeasuredAt = measuredAt;
        HealthRecordId = healthRecordId;
    }
}