using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Hospitals.Commands;

namespace UserService.Application.UseCases.V1.Commands.Hospitals;

public sealed class UpdateHospitalStaffCommandHandler : ICommandHandler<UpdateHospitalStaffCommand, Success>
{
    private readonly IRepositoryBase<UserInfo, Guid> _userInfoRepository;

    public UpdateHospitalStaffCommandHandler(IRepositoryBase<UserInfo, Guid> userInfoRepository)
    {
        _userInfoRepository = userInfoRepository;
    }
    
    public async Task<Result<Success>> Handle(UpdateHospitalStaffCommand command, CancellationToken cancellationToken)
    {
        var hospitalStaffFound = await _userInfoRepository.FindSingleAsync(u => u.Id == command.UserId, true, cancellationToken);
        if (hospitalStaffFound is null)
        {
            return FailureFromMessage(HospitalStaffErrors.ProfileNotExist);
        }

        FullName? fullName = null;
        if (command.FirstName != null || command.MiddleName != null || command.LastName != null)
        {
            var firstName = command.FirstName ?? hospitalStaffFound.FullName.FirstName;
            var lastName = command.LastName ?? hospitalStaffFound.FullName.LastName;
            fullName = FullName.Create(lastName, command.MiddleName, firstName);
        }
        DateTime? dateOfBirth = null;
        if (command.DateOfBirth != null)
        {
            dateOfBirth = DateTime.SpecifyKind(command.DateOfBirth.Value, DateTimeKind.Utc);
        }
        var gender =  command.Gender.ToEnumNullable<GenderEnum, GenderType>();
        hospitalStaffFound.UpdateBasicInfo(fullName, dateOfBirth, gender);
        return Result.Success(new Success(HospitalStaffMessages.UpdateProfileSuccessfully.GetMessage().Code, HospitalStaffMessages.UpdateProfileSuccessfully.GetMessage().Message));
    }
    
    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}