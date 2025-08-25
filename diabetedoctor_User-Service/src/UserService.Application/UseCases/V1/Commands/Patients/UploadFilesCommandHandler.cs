using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Infrastructure;
using UserService.Contract.Services.Patients.Commands;
using UserService.Contract.Services.Patients.Responses;
using MediaType = UserService.Contract.Enums.MediaType;

namespace UserService.Application.UseCases.V1.Commands.Patients;

public sealed class UploadFilesCommandHandler : ICommandHandler<UploadFilesCommand, Success<IEnumerable<MediaResponse>>>
{
    private readonly IMediaService _mediaService;
    private readonly IRepositoryBase<Domain.Models.Media, Guid> _mediaRepository;

    public UploadFilesCommandHandler(IMediaService mediaService,
        IRepositoryBase<Domain.Models.Media, Guid> mediaRepository)
    {
        _mediaService = mediaService;
        _mediaRepository = mediaRepository;
    }

    public async Task<Result<Success<IEnumerable<MediaResponse>>>> Handle(UploadFilesCommand command,
        CancellationToken cancellationToken)
    {
        var result = new List<MediaResponse>();
        foreach (var fileImage in command.Images)
        {
            var mediaType = MediaType.Image;
            if (!fileImage.ContentType.StartsWith("image"))
            {
                mediaType = MediaType.File;
            }

            var imageUploaded = await _mediaService.UploadFileAsync(fileImage, mediaType);
            if (imageUploaded.Error != null)
            {
                return FailureFromMessage(MediaErrors.UploadImagesFailed);
            }

            var mediaTypeInDomain = mediaType switch
            {
                MediaType.File => Domain.Enums.MediaType.File,
                _ => Domain.Enums.MediaType.Image
            };
            var imageAddedId = Guid.NewGuid();
            var imageAdded = Domain.Models.Media.Create(imageAddedId, imageUploaded.PublicId,
                imageUploaded.SecureUrl.AbsoluteUri, mediaTypeInDomain, Guid.Parse(command.UploadedBy));
            _mediaRepository.Add(imageAdded);
            result.Add(new MediaResponse(imageAddedId.ToString(), imageUploaded.PublicId,
                imageUploaded.SecureUrl.AbsoluteUri));
        }


        return Result.Success(new Success<IEnumerable<MediaResponse>>(
            MediaMessages.UploadFilesSuccessfully.GetMessage().Code,
            MediaMessages.UploadFilesSuccessfully.GetMessage().Message, result));
    }
    
    private static Result<Success<IEnumerable<MediaResponse>>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<IEnumerable<MediaResponse>>>(error);
    }
}