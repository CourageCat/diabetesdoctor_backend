using MediaService.Contract.Common.DomainErrors;
using MediaService.Contract.Enumarations.Media;
using MediaService.Contract.Infrastructure.Services;
using MediaService.Contract.Services.Media;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.ValueObjects;
using MongoDB.Bson;

namespace MediaService.Application.UseCase.V1.Commands.Media;

public sealed class UploadFilesCommandHandler : ICommandHandler<UploadFilesCommand, Success<IEnumerable<MediaResponse>>>
{
    private readonly IMediaService _mediaService;
    private readonly IMediaRepository _mediaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UploadFilesCommandHandler(IMediaService mediaService, IMediaRepository mediaRepository, IUnitOfWork unitOfWork)
    {
        _mediaService = mediaService;
        _mediaRepository = mediaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success<IEnumerable<MediaResponse>>>> Handle(UploadFilesCommand request,
        CancellationToken cancellationToken)
    {
        var result = new List<MediaResponse>();
        await _unitOfWork.StartTransactionAsync(cancellationToken);
        try
        {
            foreach (var fileImage in request.Images)
            {
                var mediaType = MediaType.Image;
                if (!fileImage.ContentType.StartsWith("image"))
                {
                    mediaType = MediaType.File;
                }
                var imageUploaded = await _mediaService.UploadFileAsync(fileImage, mediaType);
                if (imageUploaded.Error != null)
                {
                    return Result.Failure<Success<IEnumerable<MediaResponse>>>(MediaErrors.UploadImagesFailedException);
                }

                var mediaTypeInDomain = mediaType switch
                {
                    MediaType.File => Domain.Enums.MediaType.File,
                    _ => Domain.Enums.MediaType.Image
                };
                var imageAddedId = ObjectId.GenerateNewId();
                var imageAdded = Domain.Models.Media.Create(imageAddedId, imageUploaded.PublicId,
                    imageUploaded.SecureUrl.AbsoluteUri, mediaTypeInDomain, UserId.Of(request.UploadedBy));
                await _mediaRepository.CreateAsync(_unitOfWork.ClientSession, imageAdded, cancellationToken);
                result.Add(new MediaResponse(imageAddedId.ToString(), imageUploaded.PublicId,
                    imageUploaded.SecureUrl.AbsoluteUri));
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await _unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }

        return Result.Success(new Success<IEnumerable<MediaResponse>>(
            MediaMessage.UploadFilesSuccessfully.GetMessage().Code,
            MediaMessage.UploadFilesSuccessfully.GetMessage().Message, result));
    }
}
