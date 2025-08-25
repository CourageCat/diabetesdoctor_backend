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

namespace MediaService.Application.UseCase.V1.Commands.Post;
public sealed class CreateDraftPostCommandHandler : ICommandHandler<CreateDraftPostCommand, Success<PostCreatedResponse>>
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDraftPostCommandHandler(IPostRepository postRepository, IPostCategoryRepository postCategoryRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success<PostCreatedResponse>>> Handle(CreateDraftPostCommand request, CancellationToken cancellationToken)
    {
        var moderatorFound = await _userRepository.ExistAsync(x => x.UserId.Id == request.ModeratorId && x.IsDeleted == false, cancellationToken);
        if (!moderatorFound)
        {
            return Result.Failure<Success<PostCreatedResponse>>(UserErrors.UserNotFoundException);
        }

        var postAddedId = ObjectId.GenerateNewId();
        var postAdded = Domain.Models.Post.CreateEmpty(postAddedId, UserId.Of(request.ModeratorId));
        await _unitOfWork.StartTransactionAsync(cancellationToken);

        try
        {
            await _postRepository.CreateAsync(_unitOfWork.ClientSession, postAdded, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            var result = new PostCreatedResponse{Id = postAddedId.ToString()};
            return Result.Success(new Success<PostCreatedResponse>(PostMessage.CreateDraftPostSuccessfully.GetMessage().Code, PostMessage.CreateDraftPostSuccessfully.GetMessage().Message, result));
        }
        catch (Exception ex)
        {
            await _unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }
    }
}
