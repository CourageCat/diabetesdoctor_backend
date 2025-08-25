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

public sealed class UpdatePostCommandHandler : ICommandHandler<UpdatePostCommand, Success>
{
    private readonly IPostRepository _postRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPostCategoryRepository _postCategoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePostCommandHandler(IPostRepository postRepository, ICategoryRepository categoryRepository,
        IPostCategoryRepository postCategoryRepository, IUserRepository userRepository,
        IMediaRepository mediaRepository, IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _categoryRepository = categoryRepository;
        _postCategoryRepository = postCategoryRepository;
        _userRepository = userRepository;
        _mediaRepository = mediaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
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

        if (postFound.ModeratorId.Id != request.ModeratorId)
        {
            return Result.Failure<Success>(PostErrors.PostIsNotBelongToStaffException);
        }

        var update = new List<UpdateDefinition<Domain.Models.Post>>();
        update.Add(Builders<Domain.Models.Post>.Update.Set(x => x.ModifiedDate, DateTime.UtcNow));
        if (!string.IsNullOrEmpty(request.Title))
        {
            update.Add(Builders<Domain.Models.Post>.Update.Set(x => x.Title, request.Title));
            update.Add(Builders<Domain.Models.Post>.Update.Set(x => x.TitleNormalize, Normalize.GetNormalizeString(request.Title)));
        }
        if (!string.IsNullOrEmpty(request.Content))
        {
            update.Add(Builders<Domain.Models.Post>.Update.Set(x => x.Content, request.Content));
        }
        if (!string.IsNullOrEmpty(request.ContentHtml))
        {
            update.Add(Builders<Domain.Models.Post>.Update.Set(x => x.ContentHtml, request.ContentHtml));
        }
        if (!string.IsNullOrEmpty(request.Content))
        {
            update.Add(Builders<Domain.Models.Post>.Update.Set(x => x.Content, request.Content));
        }

        // imagesUsed contain the images that will be used in Post
        var imagesUsed = new List<ObjectId>();
        if (!string.IsNullOrEmpty(request.Thumbnail))
        {
            var thumbnailFound = await _mediaRepository.FindSingleAsync(media => media.Id == ObjectId.Parse(request.Thumbnail), cancellationToken);
            if (thumbnailFound is null)
            {
                return Result.Failure<Success>(MediaErrors.ImageNotFoundException);
            }
            update.Add(Builders<Domain.Models.Post>.Update.Set(x => x.Thumbnail, Image.Of(thumbnailFound.PublicId, thumbnailFound.PublicUrl)));
            imagesUsed.Add(thumbnailFound.Id);
        }
        if (request.Images != null && request.Images.Count != 0)
        {
            var imagesObjectId = request.Images.Select(ObjectId.Parse).ToList();
            var imagesFound = (await _mediaRepository.FindListAsync(media => imagesObjectId.Contains(media.Id), cancellationToken)).ToList();
            if (imagesFound.Count != request.Images.Count)
            {
                return Result.Failure<Success>(MediaErrors.ImagesNotFoundException);
            }

            if (!string.IsNullOrEmpty(request.Thumbnail))
            {
                imagesObjectId.Add(ObjectId.Parse(request.Thumbnail));
            }
            update.Add(Builders<Domain.Models.Post>.Update.Set(x => x.Images, imagesObjectId));
            imagesUsed.AddRange(imagesObjectId);
        }
        
        if (!string.IsNullOrEmpty(request.DoctorId))
        {
            var doctorFound = await _userRepository.ExistAsync(user => user.UserId.Id == request.DoctorId, cancellationToken);
            if (!doctorFound)
            {
                return Result.Failure<Success>(UserErrors.UserNotFoundException);
            }
            update.Add(Builders<Domain.Models.Post>.Update.Set(x => x.DoctorId, UserId.Of(request.DoctorId)));
        }
        
        if (!request.IsDraft)
        {
            update.Add(Builders<Domain.Models.Post>.Update.Set(x => x.Status, PostStatus.Pending));
        }

        var postCategoriesAdded = new List<PostCategory>();
        // Post Categories
        if (request.CategoryIds != null && request.CategoryIds.Count != 0)
        {
            var categoryIds = request.CategoryIds.Select(x => ObjectId.Parse(x));
            var existedCategories =
                (await _categoryRepository.FindListAsync(x => categoryIds.Contains(x.Id), cancellationToken)).ToList();
            if (existedCategories.Count == 0)
            {
                return Result.Failure<Success>(CategoryErrors.CategoriesNotFoundException);
            }

            postCategoriesAdded.AddRange(existedCategories.Select(pc =>
            {
                var postCategoryAddedId = ObjectId.GenerateNewId();
                return PostCategory.Create(postCategoryAddedId, postFound.Id, pc.Id);
            }).ToList());
        }
        
        var updateDefinition = Builders<Domain.Models.Post>.Update.Combine(update);

        await _unitOfWork.StartTransactionAsync(cancellationToken);

        try
        {
            if (request.CategoryIds != null)
            {
                await _postCategoryRepository.CreateManyAsync(_unitOfWork.ClientSession, postCategoriesAdded,
                    cancellationToken);
            }

            if (imagesUsed.Count != 0)
            {
                var updateImagesDefinition = Builders<Domain.Models.Media>.Update
                    .Set(image => image.ExpiredAt, null)
                    .Set(image => image.IsUsed, true)
                    .Set(image => image.ModifiedDate, DateTime.UtcNow);
                await _mediaRepository.UpdateManyAsync(_unitOfWork.ClientSession, media => imagesUsed.Contains(media.Id), updateImagesDefinition, cancellationToken);
            }
            await _postRepository.UpdateOneAsync(_unitOfWork.ClientSession, request.Id, updateDefinition,
                cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            
            
            // Return Result
            return Result.Success(new Success(PostMessage.UpdatePostSuccessfully.GetMessage().Code,
                PostMessage.UpdatePostSuccessfully.GetMessage().Message));
        }
        catch (Exception ex)
        {
            await _unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }
    }
}