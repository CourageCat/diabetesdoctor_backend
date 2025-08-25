using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Application.UseCases.V1.Commands.Patients;

public sealed class UpdatePatientProfileCommandHandler : ICommandHandler<UpdatePatientProfileCommand, Success>
{
    private readonly IRepositoryBase<UserInfo, Guid> _userInfoRepository;

    public UpdatePatientProfileCommandHandler(IRepositoryBase<UserInfo, Guid> userInfoRepository)
    {
        _userInfoRepository = userInfoRepository;
    }
    
    public async Task<Result<Success>> Handle(UpdatePatientProfileCommand command, CancellationToken cancellationToken)
    {
        var patientFound = await _userInfoRepository.FindSingleAsync(u => u.Id == command.UserId, true, cancellationToken);
        if (patientFound is null)
        {
            return FailureFromMessage(PatientErrors.ProfileNotExist);
        }

        FullName? fullName = null;
        if (command.FirstName != null || command.MiddleName != null || command.LastName != null)
        {
            var firstName = command.FirstName ?? patientFound.FullName.FirstName;
            var lastName = command.LastName ?? patientFound.FullName.LastName;
            fullName = FullName.Create(lastName, command.MiddleName, firstName);
        }
        DateTime? dateOfBirth = null;
        if (command.DateOfBirth != null)
        {
            dateOfBirth = DateTime.SpecifyKind(command.DateOfBirth.Value, DateTimeKind.Utc);
        }
        var gender =  command.Gender.ToEnumNullable<GenderEnum, GenderType>();
        patientFound.UpdateBasicInfo(fullName, dateOfBirth, gender);
        return Result.Success(new Success(PatientMessages.UpdateProfileSuccessfully.GetMessage().Code, PatientMessages.UpdateProfileSuccessfully.GetMessage().Message));
    }
    
    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}