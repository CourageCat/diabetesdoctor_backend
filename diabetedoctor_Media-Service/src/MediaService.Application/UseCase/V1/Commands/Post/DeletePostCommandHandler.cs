using LinqKit;
using MediaService.Application.Helper;
using MediaService.Contract.Common.Constant.Event;
using MediaService.Contract.Common.DomainErrors;
using MediaService.Contract.Enumarations.Post;
using MediaService.Contract.EventBus.Events.PostIntegrationEvents;
using MediaService.Contract.Infrastructure.Services;
using MediaService.Contract.Services.Post;
using MediaService.Domain.Abstractions;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.Enums;
using MediaService.Domain.Models;
using MediaService.Domain.ValueObjects;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MediaService.Application.UseCase.V1.Commands.Post;

public sealed class DeletePostCommandHandler : ICommandHandler<DeletePostCommand, Success>
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly IMediaService _mediaService;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePostCommandHandler(IPostRepository postRepository, IUserRepository userRepository, IMediaRepository mediaRepository, IMediaService mediaService, 
        IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _mediaRepository = mediaRepository;
        _mediaService = mediaService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var postFound = await _postRepository.FindSingleAsync(x => x.Id == request.Id, cancellationToken);
        if (postFound is null)
        {
            return Result.Failure<Success>(PostErrors.PostNotFound);
        }

        if (postFound.Status != PostStatus.Drafted)
        {
            return Result.Failure<Success>(PostErrors.PostHasAlreadyBeenPublishedException);
        }

        var moderatorFound =
            await _userRepository.ExistAsync(x => x.UserId.Id == request.ModeratorId && x.IsDeleted == false,
                cancellationToken);
        if (!moderatorFound)
        {
            return Result.Failure<Success>(UserErrors.UserNotFoundException);
        }

        if (postFound.ModeratorId == UserId.Of(request.ModeratorId))
        {
            return Result.Failure<Success>(PostErrors.PostIsNotBelongToStaffException);
        }

        var imagesDeleted = new List<Domain.Models.Media>();
        if (postFound.Images.Count != 0)
        {
            imagesDeleted =
                (await _mediaRepository.FindListAsync(image => postFound.Images.Contains(image.Id), cancellationToken))
                .ToList();
            if (imagesDeleted.Count != 0)
            {
                await _mediaService.DeleteFilesAsync(imagesDeleted.Select(image => image.PublicId).ToArray());
            }
        }

        await _unitOfWork.StartTransactionAsync(cancellationToken);

        try
        {
            await _postRepository.DeleteAsync(_unitOfWork.ClientSession, request.Id,
                cancellationToken);
            if (postFound.Images.Count != 0 && imagesDeleted.Count != 0)
            {
                await _mediaRepository.DeleteManyAsync(_unitOfWork.ClientSession, imagesDeleted.Select(image => image.Id), cancellationToken);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return Result.Success(new Success(PostMessage.DeletePostSuccessfully.GetMessage().Code,
                PostMessage.DeletePostSuccessfully.GetMessage().Message));
        }
        catch (Exception ex)
        {
            await _unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }
    }
}