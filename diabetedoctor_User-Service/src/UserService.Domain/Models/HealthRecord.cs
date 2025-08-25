namespace UserService.Domain.Models;

/// <summary>
/// DomainEntity thể hiện từng chỉ số sức khỏe (đường huyết, huyết áp...) của bệnh nhân.
/// </summary>
public sealed class HealthRecord : DomainEntity<Guid>
{
    // ID hồ sơ bệnh nhân
    public Guid PatientProfileId { get; private set; }

    // Loại chỉ số sức khỏe (đường huyết, huyết áp, v.v.)
    public RecordType RecordType { get; private set; }

    // Giá trị chỉ số (value object: có thể là SingleValue, BloodPressureValue...)
    public HealthRecordValue RecordValue { get; private set; }

    // Thời điểm đo (có thể cùng ngày, khác giờ)
    public DateTime MeasuredAt { get; private set; }

    // Ghi chú của người dùng
    public string? PersonNote { get; private set; }
    public string? AssistantNote { get; private set; }
    public string? DoctorNote { get; private set; }

    // Lịch đo đường huyết từng ngày
    public CarePlanMeasurementInstance? CarePlanMeasurementInstance { get; private set; } = null!;

    private HealthRecord() { }

    private HealthRecord(Guid patientId,
        RecordType recordType,
        HealthRecordValue recordValue,
        DateTime measuredAt,
        string? note = null,
        string? assistantNote = null,
        Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        PatientProfileId = patientId;
        RecordType = recordType;
        RecordValue = recordValue ?? throw new ArgumentNullException(nameof(recordValue));
        MeasuredAt = measuredAt;
        PersonNote = note?.Trim();
        AssistantNote = assistantNote?.Trim();
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = DateTime.UtcNow;
        IsDeleted = false;
    }

    public static HealthRecord Create(Guid patientId, RecordType recordType, HealthRecordValue recordValue, DateTime measuredAt, string? personNote = null, string? assistantNote = null, Guid? id = null)
        => new(patientId, recordType, recordValue, measuredAt, personNote, assistantNote, id);

    public void UpdateRecord(HealthRecordValue newValue, DateTime measuredAt, string? note = null)
    {
        RecordValue = newValue ?? throw new ArgumentNullException(nameof(newValue));
        MeasuredAt = measuredAt;
        PersonNote = note?.Trim();
    }

    public void UpdateAiNote(string note)
    {
        AssistantNote = note?.Trim();
    }
}
