using ChatService.Contract.Infrastructure.Services;
using ChatService.Contract.Services.Conversation.IntegrationCommands;

namespace ChatService.Application.UseCase.V1.IntegrationCommands.Conversations.GroupConversation;

public sealed class DeleteOldGroupAvatarCommandHandler(
    IUnitOfWork unitOfWork,
    IMediaRepository mediaRepository,
    ICloudinaryService cloudinary)
    : ICommandHandler<DeleteOldGroupAvatarCommand>
{
    public async Task<Result> Handle(DeleteOldGroupAvatarCommand request, CancellationToken cancellationToken)
    {
        var isMediaExisted = await mediaRepository.ExistsAsync(m => m.PublicId == request.ImagePublicId, cancellationToken);
        if (!isMediaExisted)
        {
            return Result.Success();
        }
        await mediaRepository.DeleteOneAsync(unitOfWork.ClientSession, request.ImagePublicId, cancellationToken);
        await cloudinary.DeleteAsync(request.ImagePublicId);
        return Result.Success();
    }
}