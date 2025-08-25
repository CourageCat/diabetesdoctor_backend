using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Enums.Doctor;
using UserService.Contract.Services.Hospitals.Commands;

namespace UserService.Application.UseCases.V1.Commands.Hospitals;

/// <summary>
/// Xử lý việc tạo hồ sơ bệnh nhân từ lệnh đầu vào, bao gồm tạo thông tin bệnh nhân, tình trạng tiểu đường,
/// tiền sử bệnh nền, ghi chú sức khỏe và lưu trữ vào cơ sở dữ liệu.
/// </summary>
public sealed class  CreateDoctorCommandHandler : ICommandHandler<CreateDoctorCommand, Success>
{
    private readonly IRepositoryBase<HospitalStaff, Guid> _hospitalStaffRepository;
    private readonly IRepositoryBase<DoctorProfile, Guid> _doctorProfileRepo;
    private readonly IRepositoryBase<UserInfo, Guid> _userInfoRepo;
    private readonly IRepositoryBase<Media, Guid> _mediaRepository;

    public CreateDoctorCommandHandler(IRepositoryBase<HospitalStaff, Guid> hospitalStaffRepository,
        IRepositoryBase<DoctorProfile, Guid> doctorProfileRepo, IRepositoryBase<UserInfo, Guid> userInfoRepo,
        IRepositoryBase<Media, Guid> mediaRepository)
    {
        _hospitalStaffRepository = hospitalStaffRepository;
        _doctorProfileRepo = doctorProfileRepo;
        _userInfoRepo = userInfoRepo;
        _mediaRepository = mediaRepository;
    }

    public async Task<Result<Success>> Handle(CreateDoctorCommand command, CancellationToken cancellationToken)
    {
        // Bước 1: Kiểm tra request
        var existingPhoneNumber = await _userInfoRepo.AnyAsync(
            u => u.PhoneNumber == command.PhoneNumber, cancellationToken);

        if (existingPhoneNumber)
            return FailureFromMessage(DoctorErrors.PhoneNumberAlreadyExists);
        var hospitalStaffFound =
            await _hospitalStaffRepository.FindSingleAsync(staff => staff.UserId == command.HospitalStaffId, true,
                cancellationToken);
        if (hospitalStaffFound is null)
        {
            return FailureFromMessage(HospitalStaffErrors.HospitalStaffNotFound);
        }

        var doctorAvatarFound =
            await _mediaRepository.FindSingleAsync(image => image.Id == command.AvatarId, true, cancellationToken);
        if (doctorAvatarFound is null)
        {
            return FailureFromMessage(MediaErrors.ImageNotFound);
        }
        
        // Bước 2: Tạo UserInfo
        var avatar = Image.Of(doctorAvatarFound.PublicId, doctorAvatarFound.PublicUrl);
        var userInfo = CreateUserInfoFromCommand(command, command.PhoneNumber, avatar, doctorAvatarFound, hospitalStaffFound.HospitalProfileId);
        _userInfoRepo.Add(userInfo);

        // Bước 3: Tạo DoctorProfile
        var doctorProfile = CreateDoctorProfileFromCommand(command, hospitalStaffFound.Id, hospitalStaffFound.HospitalProfileId, userInfo.Id);

        // Bước 4: Lưu xuống Database
        _doctorProfileRepo.Add(doctorProfile);
        
        // Cập nhật avatar của doctor thành đã được sử dụng
        doctorAvatarFound.Used();
        _mediaRepository.Update(doctorAvatarFound);

        // Bước 5: Trả về kết quả thành công
        return Result.Success(new Success(
            DoctorMessages.CreateDoctorSuccessfully.GetMessage().Code,
            DoctorMessages.CreateDoctorSuccessfully.GetMessage().Message));
    }

    private UserInfo CreateUserInfoFromCommand(CreateDoctorCommand command, string phoneNumber, Image avatar, Media media, Guid hospitalId)
    {
        var userId = new UuidV7().Value;
        var dateOfBirth = DateTime.SpecifyKind(command.DateOfBirth, DateTimeKind.Utc);
        // Chuyển đổi các enum từ command sang domain enum
        var gender = command.Gender.ToEnum<GenderEnum, GenderType>();
        var fullName = FullName.Create(command.LastName, command.MiddleName, command.FirstName);
        return UserInfo.CreateDoctor(userId, null, phoneNumber, avatar, fullName, dateOfBirth, gender, media, hospitalId);
    }

    /// <summary>
    /// Tạo doctorProfile
    /// </summary>
    private DoctorProfile CreateDoctorProfileFromCommand(CreateDoctorCommand command, Guid hospitalStaffId, Guid hospitalId, Guid userId)
    {
        // Chuyển đổi các enum từ command sang domain enum
        var position = command.Position.ToEnum<DoctorPositionEnum, DoctorPositionType>();

        var doctorId = new UuidV7().Value;

        // Tạo DoctorProfile aggregate root
        var doctorProfile = DoctorProfile.Create(
            doctorId,
            userId,
            hospitalId,
            hospitalStaffId,
            command.NumberOfExperiences,
            position,
            command.Introduction
        );

        return doctorProfile;
    }

    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}