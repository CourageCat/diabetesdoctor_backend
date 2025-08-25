
using NotificationService.Application.Mapping;
using NotificationService.Contract.Services.User;

namespace NotificationService.Application.UseCases.V1.IntegrationCommands.Users;


public class UpdateUserCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository) : ICommandHandler<UpdateUserCommand>
{
    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindSingleAsync(user => user.UserId.Id == request.UserId, cancellationToken: cancellationToken);
        
        if (user is null)
        {
            throw new UserException.UserNotFoundException();
        }

        var avatar = !string.IsNullOrWhiteSpace(request.Avatar) ? Image.Of("avatar", request.Avatar) : null;
        var fullname = request.FullName is not null ? Mapper.MapFullName(request.FullName).ToString() : null;

        user.Modify(fullname, avatar);
        await userRepository.ReplaceOneAsync(unitOfWork.ClientSession, user.Id, user, cancellationToken);

        return Result.Success(new Success(UserMessage.UpdateUserSuccessfully.GetMessage().Code, UserMessage.UpdateUserSuccessfully.GetMessage().Message));
    }
}