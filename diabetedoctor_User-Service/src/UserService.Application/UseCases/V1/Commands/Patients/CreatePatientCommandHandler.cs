
﻿using System.Text.Json;
using Microsoft.Extensions.Options;
using UserService.Contract.Common.Constants;
using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Infrastructure;
using UserService.Contract.Services.Patients.Commands;
using UserService.Contract.Settings;
using DiagnosisInfo = UserService.Domain.ValueObjects.DiagnosisInfo;
using MediaType = UserService.Domain.Enums.MediaType;

namespace UserService.Application.UseCases.V1.Commands.Patients;

/// <summary>
/// Xử lý việc tạo hồ sơ bệnh nhân từ lệnh đầu vào, bao gồm tạo thông tin bệnh nhân, tình trạng tiểu đường,
/// tiền sử bệnh nền, ghi chú sức khỏe và lưu trữ vào cơ sở dữ liệu.
/// </summary>
public sealed class CreatePatientCommandHandler : ICommandHandler<CreatePatientProfileCommand, Success>
{
    private readonly IRepositoryBase<PatientProfile, Guid> _patientProfileRepo;
    private readonly IRepositoryBase<UserInfo, Guid> _userInfoRepo;
    private readonly IResponseCacheService _responseCacheService;
    private readonly AppDefaultSettings _appDefaultSettings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;

    public CreatePatientCommandHandler(IRepositoryBase<PatientProfile, Guid> patientProfileRepo,
        IRepositoryBase<UserInfo, Guid> userInfoRepo, IResponseCacheService responseCacheService,
        IOptions<AppDefaultSettings> appDefaultOptions, IUnitOfWork unitOfWork, IPublisher publisher)
    {
        _patientProfileRepo = patientProfileRepo;
        _userInfoRepo = userInfoRepo;
        _responseCacheService = responseCacheService;
        _appDefaultSettings = appDefaultOptions.Value;
        _unitOfWork = unitOfWork;
        _publisher = publisher;
    }

    public async Task<Result<Success>> Handle(CreatePatientProfileCommand command, CancellationToken cancellationToken)
    {
        //Bước 1: Kiểm tra User
        var cacheKey = $"{AuthConstants.PhoneNumberCachePrefix}{command.UserId}";
        var phoneNumber = await _responseCacheService.GetCacheResponseAsync(cacheKey);
        if (phoneNumber is null)
        {
            return FailureFromMessage(PatientErrors.PhoneNumberNotRegistered);
        }
        
        phoneNumber = JsonSerializer.Deserialize<string>(phoneNumber);
        var existingUser = await _userInfoRepo.AnyAsync(u => u.Id == command.UserId, cancellationToken);
        if (existingUser)
            return FailureFromMessage(PatientErrors.ProfileExist);

        var avatar = Image.Of(_appDefaultSettings.AvatarPatientId, _appDefaultSettings.AvatarPatientUrl);
        var mediaId = new UuidV7().Value;
        var media = Media.Create(mediaId, avatar.PublicId, avatar.Url, MediaType.Image, command.UserId, command.UserId);
        media.Used();
        var userInfo = CreateUserInfoFromCommand(command, phoneNumber!, avatar, media);
        _userInfoRepo.Add(userInfo);

        var existingPatientProfile =
            await _patientProfileRepo.AnyAsync(p => p.UserId == command.UserId, cancellationToken);
        if (existingPatientProfile)
            return FailureFromMessage(PatientErrors.ProfileExist);

        // Bước 2: Tạo PatientProfile
        var patientProfile = CreatePatientProfileFromCommand(command);

        // Bước 3: Tạo HealthRecord (weight, note nếu có)
        var healthRecords = CreateHealthRecords(patientProfile.Id, command);

        // Bước 4: Thêm HealthRecord vào PatientProfile
        patientProfile.AddRangHealthRecords(healthRecords);

        // Bước 5: Lưu xuống Database
        _patientProfileRepo.Add(patientProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var domainEvent = PatientProfileCreatedDomainEvent.Create(command, patientProfile.Id);
        await _publisher.Publish(domainEvent, cancellationToken);
        // Bước 6: Xóa cache sđt
        await _responseCacheService.DeleteCacheResponseAsync(cacheKey);

        // Bước 7: Trả về kết quả thành công
        return Result.Success(new Success(
            PatientMessages.CreateProfileSuccessfully.GetMessage().Code,
            PatientMessages.CreateProfileSuccessfully.GetMessage().Message));
    }

    private UserInfo CreateUserInfoFromCommand(CreatePatientProfileCommand command, string phoneNumber, Image avatar,
        Media media)
    {
        var dateOfBirth = DateTime.SpecifyKind(command.DateOfBirth, DateTimeKind.Utc);
        // Chuyển đổi các enum từ command sang domain enum
        var gender = command.Gender.ToEnum<GenderEnum, GenderType>();
        var fullName = FullName.Create(command.LastName, command.MiddleName, command.FirstName);
        return UserInfo.CreatePatient(command.UserId, null, phoneNumber, avatar, fullName, dateOfBirth, gender, media);
    }

    /// <summary>
    /// Tạo patientProfile
    /// </summary>
    private PatientProfile CreatePatientProfileFromCommand(CreatePatientProfileCommand command)
    {
        // Chuyển đổi các enum từ command sang domain enum
        var diabetesType = command.Diabetes.ToEnum<DiabetesEnum, DiabetesType>();
        var diagnosisRecency = command.DiagnosisRecency.ToEnum<DiagnosisRecencyEnum, DiagnosisRecencyType>();

        var treatmentMethod = command.Type2TreatmentMethod.ToEnumNullable<TreatmentMethodEnum, TreatmentMethodType>();
        var controlLevel = command.ControlLevel.ToEnumNullable<ControlLevelEnum, ControlLevelType>();
        var insulinFrequency = command.InsulinInjectionFrequency
            .ToEnumNullable<InsulinInjectionFrequencyEnum, InsulinInjectionFrequencyType>();
        var exerciseFrequency =
            command.ExerciseFrequency.ToEnumNullable<ExerciseFrequencyEnum, ExerciseFrequencyType>();
        var eatingHabit = command.EatingHabit.ToEnumNullable<EatingHabitEnum, EatingHabitType>();

        // Chuyển đổi danh sách biến chứng (nếu có)
        var complications = command.Complications?
            .Select(c => c.ToEnum<ComplicationEnum, ComplicationType>())
            .ToList();

        // Tạo các value object diagnosisInfo, diabetesCondition
        var diagnosisInfo = DiagnosisInfo.Of(diagnosisRecency, command.Year);

        var diabetesCondition = DiabetesCondition.Of(
            diabetesType,
            insulinFrequency,
            treatmentMethod,
            controlLevel,
            complications?.Any() ?? false,
            complications,
            command.OtherComplicationDescription,
            exerciseFrequency,
            eatingHabit,
            command.UsesAlcoholOrTobacco
        );
        var patientId = new UuidV7().Value;
        // Tạo PatientProfile aggregate root
        var patientProfile = PatientProfile.Create(
            patientId,
            command.UserId!,
            diabetesType,
            diagnosisInfo,
            diabetesCondition,
            command
        );


        // Thêm tiền sử bệnh nền (nếu có)
        if (command.MedicalHistories?.Any() != true) return patientProfile;
        foreach (var historyEnum in command.MedicalHistories)
        {
            var history = historyEnum.ToEnum<MedicalHistoryForDiabetesEnum, MedicalHistoryForDiabetesType>();
            patientProfile.AddMedicalHistory(history);
        }

        return patientProfile;
    }

    /// <summary>
    /// Tạo HealthRecord
    /// </summary>
    private static List<HealthRecord> CreateHealthRecords(Guid patientId, CreatePatientProfileCommand command)
    {
        // Tạo cân nặng
        var weightValue = WeightValue.Of(command.WeightKg);
        var weightRecord = HealthRecord.Create(patientId, RecordType.Weight, weightValue, DateTime.UtcNow);

        // Tạo chiều cao
        var heightValue = HeightValue.Of(command.HeightCm);
        var heightRecord = HealthRecord.Create(patientId, RecordType.Height, heightValue, DateTime.UtcNow);

        return [weightRecord, heightRecord];
    }

    /// <summary>
    /// Creates a failure Result from an PatientMessages enum.
    /// </summary>
    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}
