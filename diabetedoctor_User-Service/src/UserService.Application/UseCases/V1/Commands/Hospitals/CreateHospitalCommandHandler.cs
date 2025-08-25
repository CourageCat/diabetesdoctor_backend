using Microsoft.Extensions.Options;
using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Hospitals.Commands;
using UserService.Contract.Settings;
using MediaType = UserService.Domain.Enums.MediaType;

namespace UserService.Application.UseCases.V1.Commands.Hospitals;

public sealed class CreateHospitalCommandHandler :ICommandHandler<CreateHospitalCommand, Success>
{
    private readonly IRepositoryBase<HospitalProfile, Guid> _hospitalRepository;
    private readonly IRepositoryBase<AdminProfile, Guid> _adminRepository;
    private readonly IRepositoryBase<Media, Guid> _mediaRepository;
    private readonly AppDefaultSettings _appDefaultSettings;
    private readonly IRepositoryBase<UserInfo, Guid> _userInfoRepository;
    private readonly IRepositoryBase<HospitalAdmin, Guid> _hospitalAdminRepository;

    public CreateHospitalCommandHandler(IRepositoryBase<HospitalProfile, Guid> hospitalRepository, IRepositoryBase<AdminProfile, Guid> adminRepository, IRepositoryBase<Media, Guid> mediaRepository, IOptions<AppDefaultSettings> appDefaultConfigs, IRepositoryBase<UserInfo, Guid> userInfoRepository, IRepositoryBase<HospitalAdmin, Guid> hospitalAdminRepository)
    {
        _hospitalRepository = hospitalRepository;
        _adminRepository = adminRepository;
        _mediaRepository = mediaRepository;
        _userInfoRepository = userInfoRepository;
        _hospitalAdminRepository = hospitalAdminRepository;
        _appDefaultSettings = appDefaultConfigs.Value;
    }

    public async Task<Result<Success>> Handle(CreateHospitalCommand command, CancellationToken cancellationToken)
    {
        // Bước 1: Kiểm tra request
        var hospitalEmailFound = await _hospitalRepository.AnyAsync(hospital => hospital.Email == command.Email, cancellationToken);
        if (hospitalEmailFound)
        {
            return FailureFromMessage(HospitalErrors.EmailAlreadyExists);
        }
        var phoneNumberFound = await _hospitalRepository.AnyAsync(hospital => hospital.PhoneNumber == command.PhoneNumber, cancellationToken);
        if (phoneNumberFound)
        {
            return FailureFromMessage(HospitalErrors.PhoneNumberAlreadyExists);
        }
        var adminFound = await _adminRepository.FindSingleAsync(admin => admin.UserId == command.AdminId, true, cancellationToken);
        if (adminFound is null)
        {
            return FailureFromMessage(AdminErrors.AdminNotFound);
        }
        var hospitalThumbnailFound = await _mediaRepository.FindSingleAsync(media => media.Id == command.Thumbnail, true, cancellationToken);
        if (hospitalThumbnailFound is null)
        {
            return FailureFromMessage(MediaErrors.ImageNotFound);
        }
        var hospitalImagesFound = (await _mediaRepository.FindAllAsync(media => command.Images.Contains(media.Id), cancellationToken)).ToList();
        if (hospitalImagesFound.Count != command.Images.Count)
        {
            return FailureFromMessage(MediaErrors.ImagesNotFound);
        }
        // Bước 2: Tạo HospitalProfile
        var thumbnail = Image.Of(hospitalThumbnailFound.PublicId, hospitalThumbnailFound.PublicUrl);
        hospitalImagesFound.Add(hospitalThumbnailFound);
        hospitalImagesFound.ForEach(image => image.Used());
        var hospitalProfile = CreateHospitalProfileFromCommand(command, adminFound.Id, thumbnail, hospitalImagesFound);
        
        // Bước 2: Tạo UserInfo
        var avatar = Image.Of(_appDefaultSettings.AvatarAdminId, _appDefaultSettings.AvatarAdminUrl);
        var mediaId = new UuidV7().Value;
        var media = Media.Create(mediaId, avatar.PublicId, avatar.Url, MediaType.Image, command.AdminId, adminFound.Id);
        media.Used();
        var userInfo = CreateUserInfoFromCommand(command, avatar, media, hospitalProfile.Id);
        _userInfoRepository.Add(userInfo);

        // Bước 3: Tạo Hospital Admin Profile
        var hospitalAdminProfile = CreateHospitalAdminFromCommand(adminFound.Id, hospitalProfile.Id, userInfo.Id);
        _hospitalAdminRepository.Add(hospitalAdminProfile);
        // Bước 4: Lưu xuống Database
        _hospitalRepository.Add(hospitalProfile);
        
        // Cập nhật avatar của hospital thành đã được sử dụng
        hospitalThumbnailFound.Used();
        _mediaRepository.Update(hospitalThumbnailFound);

        // Bước 5: Trả về kết quả thành công
        return Result.Success(new Success(
            HospitalMessages.CreateHospitalSuccessfully.GetMessage().Code,
            HospitalMessages.CreateHospitalSuccessfully.GetMessage().Message));
    }
    
    private HospitalProfile CreateHospitalProfileFromCommand(CreateHospitalCommand command, Guid adminId, Image thumbnail, List<Media> medias)
    {
        var hospitalId = new UuidV7().Value;

        // Tạo HospitalProfile aggregate root
        var hospitalProfile = HospitalProfile.Create(
            hospitalId,
            command.Name,
            command.Email,
            command.PhoneNumber,
            command.Website,
            command.Address,
            command.Introduction,
            thumbnail,
            adminId,
            medias
        );

        return hospitalProfile;
    }
    
    private UserInfo CreateUserInfoFromCommand(CreateHospitalCommand command, Image avatar, Media media, Guid hospitalId)
    {
        var userId = new UuidV7().Value;
        var dateOfBirth = DateTime.UtcNow.AddYears(-20);
        // Chuyển đổi các enum từ command sang domain enum
        var gender = GenderType.Male;
        var fullName = FullName.Create("Admin ", "", command.Name);
        return UserInfo.CreateHospitalAdmin(userId, command.Email, null, avatar, fullName, dateOfBirth, gender, media, hospitalId);
    }
    
    private HospitalAdmin CreateHospitalAdminFromCommand(Guid adminId, Guid hospitalId, Guid userId)
    {
        var hospitalAdminId = new UuidV7().Value;

        // Tạo HospitalStaffProfile aggregate root
        var hospitalAdminProfile = HospitalAdmin.Create(
            hospitalAdminId,
            userId,
            hospitalId,
            adminId
        );

        return hospitalAdminProfile;
    }
    
    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}