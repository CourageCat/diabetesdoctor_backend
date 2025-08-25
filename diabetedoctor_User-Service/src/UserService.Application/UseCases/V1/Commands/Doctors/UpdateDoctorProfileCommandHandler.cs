using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Enums.Doctor;
using UserService.Contract.Services.Doctors.Commands;

namespace UserService.Application.UseCases.V1.Commands.Doctors;

public sealed class UpdateDoctorProfileCommandHandler : ICommandHandler<UpdateDoctorProfileCommand, Success>
{
    private readonly IRepositoryBase<UserInfo, Guid> _userInfoRepository;
    private readonly IRepositoryBase<DoctorProfile, Guid> _doctorProfileRepository;

    public UpdateDoctorProfileCommandHandler(IRepositoryBase<UserInfo, Guid> userInfoRepository, IRepositoryBase<DoctorProfile, Guid> doctorProfileRepository)
    {
        _userInfoRepository = userInfoRepository;
        _doctorProfileRepository = doctorProfileRepository;
    }
    
    public async Task<Result<Success>> Handle(UpdateDoctorProfileCommand command, CancellationToken cancellationToken)
    {
        // Update UserInfo
        var userFound = await _userInfoRepository.FindSingleAsync(u => u.Id == command.UserId, true, cancellationToken);
        if (userFound is null)
        {
            return FailureFromMessage(DoctorErrors.ProfileNotExist);
        }
        var doctorFound = await _doctorProfileRepository.FindSingleAsync(u => u.UserId == command.UserId, true, cancellationToken);
        if (doctorFound is null)
        {
            return FailureFromMessage(DoctorErrors.ProfileNotExist);
        }

        FullName? fullName = null;
        if (command.FirstName != null || command.MiddleName != null || command.LastName != null)
        {
            var firstName = command.FirstName ?? userFound.FullName.FirstName;
            var lastName = command.LastName ?? userFound.FullName.LastName;
            fullName = FullName.Create(lastName, command.MiddleName, firstName);
        }
        DateTime? dateOfBirth = null;
        if (command.DateOfBirth != null)
        {
            dateOfBirth = DateTime.SpecifyKind(command.DateOfBirth.Value, DateTimeKind.Utc);
        }
        var gender =  command.Gender.ToEnumNullable<GenderEnum, GenderType>();
        var position = command.Position.ToEnumNullable<DoctorPositionEnum, DoctorPositionType>();
        userFound.UpdateBasicInfo(fullName, dateOfBirth, gender);
        doctorFound.UpdateBasicInfo(command.NumberOfExperiences, position, command.Introduction);
        return Result.Success(new Success(DoctorMessages.UpdateProfileSuccessfully.GetMessage().Code, DoctorMessages.UpdateProfileSuccessfully.GetMessage().Message));
    }
    
    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}