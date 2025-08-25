using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Infrastructure;
using UserService.Contract.Services.Patients.Commands;
using MediaType = UserService.Contract.Enums.MediaType;

namespace UserService.Application.UseCases.V1.Commands.Patients;

public sealed class ChangeAvatarCommandHandler : ICommandHandler<ChangeAvatarCommand, Success>
{
    private readonly IMediaService _mediaService;
    private readonly IRepositoryBase<UserInfo, Guid> _userRepository;
    private readonly IRepositoryBase<Media, Guid> _mediaRepository;

    public ChangeAvatarCommandHandler(IMediaService mediaService, IRepositoryBase<UserInfo, Guid> userRepository,
        IRepositoryBase<Media, Guid> mediaRepository)
    {
        _mediaService = mediaService;
        _userRepository = userRepository;
        _mediaRepository = mediaRepository;
    }

    public async Task<Result<Success>> Handle(ChangeAvatarCommand command, CancellationToken cancellationToken)
    {
        var userFound = await _userRepository.FindSingleAsync(user => user.Id == command.UserId, true, cancellationToken);
        if (userFound is null)
        {
            switch (command.Role)
            {
                case nameof(RoleType.Patient):
                    return FailureFromMessage(PatientErrors.ProfileNotExist);
                case nameof(RoleType.Doctor):
                    return FailureFromMessage(DoctorErrors.ProfileNotExist);
                default:
                    return FailureFromMessage(HospitalStaffErrors.ProfileNotExist);
            }
        }

        var imageUploaded = await _mediaService.UploadFileAsync(command.Avatar, MediaType.Image);
        if (imageUploaded.Error != null)
        {
            return FailureFromMessage(PatientErrors.ChangeAvatarFailed);
        }

        // Delete directly image in Media and do not delete in Cloudinary if User is a Patient and User first updated avatar
        var oldAvatar = await _mediaRepository.FindFirstAsync(
            media => media.UserInfoId == userFound.Id,
            cancellationToken
        );
        var shouldDeleteDirectly = command.Role == nameof(RoleType.Patient) && !userFound.IsFirstUpdatedAvatar;

        if (oldAvatar is not null)
        {
            if (shouldDeleteDirectly)
            {
                _mediaRepository.Remove(oldAvatar);
            }
            else
            {
                oldAvatar.UnUsed();
            }
        }


        var imageId = Guid.NewGuid();
        var imageUpdated = Media.Create(imageId, imageUploaded.PublicId,
            imageUploaded.SecureUrl.AbsoluteUri, Domain.Enums.MediaType.Image, command.UserId, command.UserId);
        imageUpdated.Used();
        userFound.UpdateAvatar(imageUpdated);
        _mediaRepository.Add(imageUpdated);
        return command.Role switch
        {
            nameof(RoleType.Patient) => Result.Success(new Success(
                PatientMessages.ChangeAvatarSuccessfully.GetMessage().Code,
                PatientMessages.ChangeAvatarSuccessfully.GetMessage().Message)),
            nameof(RoleType.Doctor) => Result.Success(new Success(
                DoctorMessages.ChangeAvatarSuccessfully.GetMessage().Code,
                DoctorMessages.ChangeAvatarSuccessfully.GetMessage().Message)),
            _ => Result.Success(new Success(
                HospitalStaffMessages.ChangeAvatarSuccessfully.GetMessage().Code,
                HospitalStaffMessages.ChangeAvatarSuccessfully.GetMessage().Message))
        };
    }

    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}