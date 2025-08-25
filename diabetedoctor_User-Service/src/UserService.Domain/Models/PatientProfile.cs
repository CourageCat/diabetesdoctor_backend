using UserService.Contract.Services.Patients.Commands;

namespace UserService.Domain.Models;

/// <summary>
/// Aggregate gốc đại diện cho hồ sơ bệnh nhân tiểu đường.
/// Gồm thông tin cá nhân, loại bệnh tiểu đường, tiền sử bệnh, và các bản ghi sức khỏe.
/// </summary>
public sealed class PatientProfile : AggregateRoot<Guid>
{
    // Thông tin cá nhân
    public Guid UserId { get; private set; }

    // Chẩn đoán & phân loại
    public DiabetesType DiabetesType { get; private set; }
    public DiagnosisInfo DiagnosisInfo { get; private set; }
    public DiabetesCondition DiabetesCondition { get; private set; }

    public UserInfo User { get; private set; } = null!;
    // Tiền sử bệnh nền
    public List<MedicalHistoryForDiabetesType> MedicalHistories { get; private set; } = [];

    // Bản ghi sức khỏe
    private readonly List<HealthRecord> _healthRecords = [];
    public IReadOnlyCollection<HealthRecord> HealthRecords => _healthRecords.AsReadOnly();

    // Mẫu lịch đo đường huyết
    private readonly List<CarePlanMeasurementTemplate> _carePlanMeasurementTemplates = [];
    public IReadOnlyCollection<CarePlanMeasurementTemplate> CarePlanMeasurementTemplates => _carePlanMeasurementTemplates.AsReadOnly();

    // Lịch đo đường huyết từng ngày
    private readonly List<CarePlanMeasurementInstance> _carePlanMeasurementInstances = [];
    public IReadOnlyCollection<CarePlanMeasurementInstance> CarePlanMeasurementInstances => _carePlanMeasurementInstances.AsReadOnly();

    private PatientProfile() { } 

    private PatientProfile(
        Guid id,
        Guid userId,
        DiabetesType diabetesType,
        DiagnosisInfo diagnosisInfo,
        DiabetesCondition diabetesCondition)
    {
        Id = id;
        UserId = userId;
        DiabetesType = diabetesType;
        DiagnosisInfo = diagnosisInfo ?? throw new ArgumentNullException(nameof(diagnosisInfo));
        DiabetesCondition = diabetesCondition ?? throw new ArgumentNullException(nameof(diabetesCondition));
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = DateTime.UtcNow;
        IsDeleted = false;
    }

    // Tạo thông tin
    public static PatientProfile Create
        (Guid id, 
        Guid userId,
        DiabetesType diabetesType,
        DiagnosisInfo diagnosisInfo,
        DiabetesCondition diabetesCondition, CreatePatientProfileCommand patientProfileCommand)
    {
        var patientProfile =  new PatientProfile(id, userId, diabetesType, diagnosisInfo, diabetesCondition);
        
         //var domainEvent = PatientProfileCreatedDomainEvent.Create(patientProfileCommand, id);
         //patientProfile.AddDomainEvent(domainEvent);
        
        return patientProfile;
    }
    
    public static PatientProfile CreateForSeedData
    (Guid id, 
        Guid userId,
        DiabetesType diabetesType,
        DiagnosisInfo diagnosisInfo,
        DiabetesCondition diabetesCondition)
    {
        var patientProfile =  new PatientProfile(id, userId, diabetesType, diagnosisInfo, diabetesCondition);
        return patientProfile;
    }

    // Cập nhật loại bệnh
    public void UpdateDiabetesType(DiabetesType diabetesType)
    {
        DiabetesType = diabetesType;
    }

    // Cập nhật năm chẩn đoán
    public void UpdateDiagnosisInfo(DiagnosisInfo diagnosisInfo)
    {
        ArgumentNullException.ThrowIfNull(diagnosisInfo);
        DiagnosisInfo = diagnosisInfo;
    }

    // Cập nhật tình trạng điều trị
    public void UpdateDiabetesCondition(DiabetesCondition condition)
    {
        ArgumentNullException.ThrowIfNull(condition);

        if (DiabetesType == DiabetesType.Type1 && condition.InsulinFrequency == null)
            throw new InvalidOperationException("Tuýp 1 phải có tần suất tiêm Insulin");

        if (DiabetesType == DiabetesType.Type2 && condition.Type2TreatmentMethod == null)
            throw new InvalidOperationException("Tuýp 2 phải có phương pháp điều trị");

        DiabetesCondition = condition;
    }

    // Thêm/xóa bệnh nền
    public void AddMedicalHistory(MedicalHistoryForDiabetesType history)
    {
        ArgumentNullException.ThrowIfNull(history);
        if (!MedicalHistories.Contains(history))
            MedicalHistories.Add(history);
    }

    public void RemoveMedicalHistory(MedicalHistoryForDiabetesType history)
    {
        ArgumentNullException.ThrowIfNull(history);
        MedicalHistories.Remove(history);
    }

    // Thêm một bản ghi sức khỏe
    public void AddHealthRecord(HealthRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        if (_healthRecords.Any(r => r.Id == record.Id))
            throw new InvalidOperationException("Duplicate HealthRecord ID");
        
        _healthRecords.Add(record);
    }

    // Thêm một bản ghi sức khỏe
    public void AddRangHealthRecords(List<HealthRecord> records)
    {
        _healthRecords.AddRange(records);
    }

    // Lấy cân nặng gần nhất (nếu cần)
    public double? GetLatestWeight()
    {
        return _healthRecords
            .Where(r => r.RecordType == RecordType.Weight)
            .OrderByDescending(r => r.MeasuredAt)
            .Select(r => ((BloodGlucoseValue)r.RecordValue).Value)
            .FirstOrDefault();
    }
}
