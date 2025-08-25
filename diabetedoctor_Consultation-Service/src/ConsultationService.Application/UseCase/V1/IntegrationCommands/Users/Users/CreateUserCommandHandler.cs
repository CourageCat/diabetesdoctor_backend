using ConsultationService.Application.Mapping;
using ConsultationService.Contract.Services.User.Commands;

namespace ConsultationService.Application.UseCase.V1.IntegrationCommands.Users.Users;

public class CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateUserCommand>
{
    public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = MapToUser(request);
        await userRepository.CreateAsync(unitOfWork.ClientSession, user, cancellationToken);
        return Result.Success();
    }

    private User MapToUser(CreateUserCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var id = ObjectId.GenerateNewId();
        var userId = UserId.Of(command.Id);
        var hospitalId = !string.IsNullOrWhiteSpace(command.HospitalId) ? HospitalId.Of(command.HospitalId) : null;
        var fullName = Mapper.MapFullName(command.FullName);
        var avatar = Image.Of("avatar", command.Avatar);
        var role = Mapper.MapRoleFromInt(command.Role);
        return User.Create(id, userId, hospitalId, fullName, avatar, command.PhoneNumber, command.Email, role);
    }
}