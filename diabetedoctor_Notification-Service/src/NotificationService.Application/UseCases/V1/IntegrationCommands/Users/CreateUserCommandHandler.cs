using NotificationService.Application.Mapping;
using NotificationService.Contract.Services.User;
using NotificationService.Domain.Models;

namespace NotificationService.Application.UseCases.V1.IntegrationCommands.Users;

public sealed class CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork) : ICommandHandler<CreateUserCommand>
{
    public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = MapToUser(request);
        await userRepository.CreateAsync(unitOfWork.ClientSession, user, cancellationToken: cancellationToken);
        return Result.Success();
    }
    
    private User MapToUser(CreateUserCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var id = ObjectId.GenerateNewId();
        var userId = UserId.Of(command.UserId);
        var avatar = Image.Of("avatar",command.Avatar);
        var fullname = Mapper.MapFullName(command.FullName);
        return User.Create(id, userId, fullname.ToString(), avatar);
    }
}