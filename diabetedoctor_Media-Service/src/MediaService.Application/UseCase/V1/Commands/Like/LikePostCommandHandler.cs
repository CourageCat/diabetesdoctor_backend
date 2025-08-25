using MediaService.Contract.Common.DomainErrors;
using MediaService.Contract.Services.Like;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.Models;
using MediaService.Domain.ValueObjects;
using MongoDB.Bson;

namespace MediaService.Application.UseCase.V1.Commands.Like;
public sealed class LikePostCommandHandler : ICommandHandler<LikePostCommand, Success>
{
    private readonly ILikeRepository _postLikeRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LikePostCommandHandler(ILikeRepository postLikeRepository, IPostRepository postRepository, IUnitOfWork unitOfWork)
    {
        _postLikeRepository = postLikeRepository;
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success>> Handle(LikePostCommand request, CancellationToken cancellationToken)
    {
        var postFound = await _postRepository.ExistAsync(x => x.Id == request.PostId, cancellationToken);
        if (!postFound)
        {
            return Result.Failure<Success>(PostErrors.PostNotFound);
        }
        var likedPostFound = await _postLikeRepository.FindSingleAsync(x => x.PostId == request.PostId && x.UserId == UserId.Of(request.UserId), cancellationToken);
        try
        {
            await _unitOfWork.StartTransactionAsync(cancellationToken);
            // User has not already liked post => Like post
            if (likedPostFound is null)
            {
                var likedPostId = ObjectId.GenerateNewId();
                var likedPost = Domain.Models.Like.Create(likedPostId, request.PostId, UserId.Of(request.UserId));
                await _postLikeRepository.CreateAsync(_unitOfWork.ClientSession, likedPost, cancellationToken);
            }
            // User has already liked post => Unlike post
            else
            {
                await _postLikeRepository.DeleteAsync(_unitOfWork.ClientSession, likedPostFound.Id, cancellationToken);
            }
            await _postRepository.UpdateLikePost(_unitOfWork.ClientSession, likedPostFound == null ? true : false, request.PostId, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await _unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }
        return Result.Success(likedPostFound == null ? new Success(LikeMessage.LikePostSuccessfully.GetMessage().Code, LikeMessage.LikePostSuccessfully.GetMessage().Message) : new Success(LikeMessage.UnLikePostSuccessfully.GetMessage().Code, LikeMessage.UnLikePostSuccessfully.GetMessage().Message));
    }
}