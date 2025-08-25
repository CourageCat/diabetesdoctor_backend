using MediaService.Application.Helper;
using MediaService.Contract.Common.Constant.Event;
using MediaService.Contract.Common.DomainErrors;
using MediaService.Contract.EventBus.Events.PostIntegrationEvents;
using MediaService.Contract.Services.Post;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.Enums;
using MongoDB.Driver;

namespace MediaService.Application.UseCase.V1.Commands.Post;

public sealed class ReviewPostCommandHandler : ICommandHandler<ReviewPostCommand, Success>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxEventRepository _outboxEventRepository;
    private readonly KafkaSettings _kafkaSettings; 
    
    public ReviewPostCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork, IOutboxEventRepository outboxEventRepository, IOptions<KafkaSettings> kafkaConfig)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
        _outboxEventRepository = outboxEventRepository;
        _kafkaSettings = kafkaConfig.Value;
    }
    
    public async Task<Result<Success>> Handle(ReviewPostCommand request, CancellationToken cancellationToken)
    {
        var postFound = await _postRepository.FindSingleAsync(p => p.Id == request.Id, cancellationToken);
        if (postFound is null)
        {
            return Result.Failure<Success>(PostErrors.PostNotFound);
        }

        if (postFound.Status == PostStatus.Drafted)
        {
            return Result.Failure<Success>(PostErrors.PostIsDraftedException);
        }

        if (postFound.Status == PostStatus.Approved || postFound.Status == PostStatus.Rejected)
        {
            return Result.Failure<Success>(PostErrors.PostHasAlreadyBeenReviewedException);
        }

        await _unitOfWork.StartTransactionAsync(cancellationToken);
        try
        {
            var update = Builders<Domain.Models.Post>.Update
                .Set(x => x.Status, request.IsApproved ? PostStatus.Approved : PostStatus.Rejected)
                .Set(x => x.ReasonRejected, request.IsApproved ? null : request.ReasonRejected)
                .Set(x => x.ModifiedDate, DateTime.UtcNow);
            await _postRepository.UpdateOneAsync(_unitOfWork.ClientSession, postFound.Id, update, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            // Publish Event
            // var integrationEvent =
            //     OutboxEventExtension.ToOutboxEvent(_kafkaSetting.PostTopic,
            //         MapToPostCreatedIntegrationEvent(postFound));
            // await _outboxEventRepository.CreateAsync(_unitOfWork.ClientSession, integrationEvent, cancellationToken);
        }
        catch (Exception e)
        {
            await _unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }

        if (request.IsApproved)
        {
            return Result.Success(new Success(PostMessage.ApprovedPostSuccessfully.GetMessage().Code, PostMessage.ApprovedPostSuccessfully.GetMessage().Message));
        }
        return Result.Success(new Success(PostMessage.RejectedPostSuccessfully.GetMessage().Code, PostMessage.RejectedPostSuccessfully.GetMessage().Message));
    }
    
    private PostCreatedIntegrationEvent MapToPostCreatedIntegrationEvent(Domain.Models.Post postModel)
    {
        return new PostCreatedIntegrationEvent
            { PostId = postModel.Id.ToString(), Title = postModel.Title, Thumbnail = postModel.Thumbnail.PublicUrl };
    }
}