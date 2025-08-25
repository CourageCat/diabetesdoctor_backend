using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Hospitals.Commands;

namespace UserService.Application.UseCases.V1.Commands.Hospitals;

public sealed class CreateHospitalStaffCommandHandler : ICommandHandler<CreateHospitalStaffCommand, Success>
{
    private readonly IRepositoryBase<HospitalStaff, Guid> _hospitalStaffRepository;
    private readonly IRepositoryBase<HospitalProfile, Guid> _hospitalRepository;
    private readonly IRepositoryBase<HospitalAdmin, Guid> _hospitalAdminRepository;
    private readonly IRepositoryBase<UserInfo, Guid> _userInfoRepo;
    private readonly IRepositoryBase<Media, Guid> _mediaRepository;

    public CreateHospitalStaffCommandHandler(IRepositoryBase<HospitalStaff, Guid> hospitalStaffRepository, IRepositoryBase<HospitalProfile, Guid> hospitalRepository, IRepositoryBase<HospitalAdmin, Guid> hospitalAdminRepository, IRepositoryBase<UserInfo, Guid> userInfoRepo, IRepositoryBase<Media, Guid> mediaRepository)
    {
        _hospitalStaffRepository = hospitalStaffRepository;
        _hospitalRepository = hospitalRepository;
        _hospitalAdminRepository = hospitalAdminRepository;
        _userInfoRepo = userInfoRepo;
        _mediaRepository = mediaRepository;
    }

    public async Task<Result<Success>> Handle(CreateHospitalStaffCommand command, CancellationToken cancellationToken)
    {
        // Bước 1: Kiểm tra request
        var existingEmail = await _userInfoRepo.AnyAsync(
            u => u.Email == command.Email, cancellationToken);
        if (existingEmail)
            return FailureFromMessage(HospitalStaffErrors.EmailAlreadyExists);
        
        var hospitalAdminFound =
            await _hospitalAdminRepository.FindSingleAsync(admin => admin.UserId == command.HospitalAdminId, true,
                cancellationToken);
        if (hospitalAdminFound is null)
        {
            return FailureFromMessage(HospitalAdminErrors.HospitalAdminNotFound);
        }
        
        var hospitalFound =
            await _hospitalRepository.FindSingleAsync(hospital => hospital.Id == hospitalAdminFound.HospitalProfileId, true,
                cancellationToken);
        if (hospitalFound is null)
        {
            return FailureFromMessage(HospitalErrors.HospitalNotFound);
        }

        var hospitalStaffAvatarFound =
            await _mediaRepository.FindSingleAsync(image => image.Id == command.AvatarId, true, cancellationToken);
        if (hospitalStaffAvatarFound is null)
        {
            return FailureFromMessage(MediaErrors.ImageNotFound);
        }
        
        // Bước 2: Tạo UserInfo
        var avatar = Image.Of(hospitalStaffAvatarFound.PublicId, hospitalStaffAvatarFound.PublicUrl);
        var userInfo = CreateUserInfoFromCommand(command, command.Email, avatar, hospitalStaffAvatarFound, hospitalFound.Id);
        _userInfoRepo.Add(userInfo);

        // Bước 3: Tạo Hospital Staff Profile
        var hospitalStaffProfile = CreateHospitalStaffProfileFromCommand(command, hospitalAdminFound.Id, hospitalFound.Id, userInfo.Id);

        // Bước 4: Lưu xuống Database
        _hospitalStaffRepository.Add(hospitalStaffProfile);
        
        // Cập nhật avatar của hospital staff thành đã được sử dụng
        hospitalStaffAvatarFound.Used();
        _mediaRepository.Update(hospitalStaffAvatarFound);

        // Bước 5: Trả về kết quả thành công
        return Result.Success(new Success(
            HospitalStaffMessages.CreateHospitalStaffSuccessfully.GetMessage().Code,
            HospitalStaffMessages.CreateHospitalStaffSuccessfully.GetMessage().Message));
    }

    private UserInfo CreateUserInfoFromCommand(CreateHospitalStaffCommand command, string email, Image avatar, Media media, Guid hospitalId)
    {
        var userId = new UuidV7().Value;
        var dateOfBirth = DateTime.SpecifyKind(command.DateOfBirth, DateTimeKind.Utc);
        // Chuyển đổi các enum từ command sang domain enum
        var gender = command.Gender.ToEnum<GenderEnum, GenderType>();
        var fullName = FullName.Create(command.LastName, command.MiddleName, command.FirstName);
        return UserInfo.CreateHospitalStaff(userId, email, null, avatar, fullName, dateOfBirth, gender, media, hospitalId);
    }

    /// <summary>
    /// Tạo hospitalStaffProfile
    /// </summary>
    private HospitalStaff CreateHospitalStaffProfileFromCommand(CreateHospitalStaffCommand command, Guid hospitalAdminId, Guid hospitalId, Guid userId)
    {
        var hospitalStaffId = new UuidV7().Value;

        // Tạo HospitalStaffProfile aggregate root
        var hospitalStaffProfile = HospitalStaff.Create(
            hospitalStaffId,
            userId,
            hospitalId,
            hospitalAdminId
        );

        return hospitalStaffProfile;
    }
    
    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}