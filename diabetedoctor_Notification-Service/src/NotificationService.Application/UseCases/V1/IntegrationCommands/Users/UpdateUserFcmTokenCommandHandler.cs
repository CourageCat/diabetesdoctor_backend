using NotificationService.Contract.Services.User;
using NotificationService.Domain.Models;

namespace NotificationService.Application.UseCases.V1.IntegrationCommands.Users;

public sealed class UpdateUserFcmTokenCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository)
    : ICommandHandler<UpdateUserFcmTokenCommand>
{
    public async Task<Result> Handle(UpdateUserFcmTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindSingleAsync(user => user.UserId.Id == request.UserId,
            cancellationToken: cancellationToken);

        if (user is null)
        {
            throw new UserException.UserNotFoundException();
        }

        var update = Builders<User>.Update.Set(x => x.FcmToken, request.FcmToken);
        await userRepository.UpdateOneAsync(unitOfWork.ClientSession, user.Id, update,
            cancellationToken: cancellationToken);
        return Result.Success();
    }
}