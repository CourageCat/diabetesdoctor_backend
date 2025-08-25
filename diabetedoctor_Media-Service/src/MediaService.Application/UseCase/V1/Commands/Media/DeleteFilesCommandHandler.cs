using MediaService.Contract.Common.DomainErrors;
using MediaService.Contract.Infrastructure.Services;
using MediaService.Contract.Services.Media;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.Enums;
using MongoDB.Bson;

namespace MediaService.Application.UseCase.V1.Commands.Media;

public sealed class DeleteFilesCommandHandler : ICommandHandler<DeleteFilesCommand, Success>
{
    private readonly IMediaService _mediaService;
    private readonly IMediaRepository _mediaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteFilesCommandHandler(IMediaService mediaService, IMediaRepository mediaRepository, IUnitOfWork unitOfWork)
    {
        _mediaService = mediaService;
        _mediaRepository = mediaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success>> Handle(DeleteFilesCommand request, CancellationToken cancellationToken)
    {
        var imagesFound = (await _mediaRepository.FindListAsync(image => request.ImageIds.Contains(image.Id.ToString()), cancellationToken)).ToList();
        if (imagesFound.Count == 0)
        {
            return Result.Failure<Success>(MediaErrors.ImagesNotFoundException);
        }
        var imageIds = imagesFound.Select(x => x.Id).ToList();
        var publicIds = imagesFound.Select(x => x.PublicId).ToArray();

        await _unitOfWork.StartTransactionAsync(cancellationToken);
        try
        {
            await _mediaRepository.DeleteManyAsync(_unitOfWork.ClientSession, imageIds, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            var resultDeletedImage = await _mediaService.DeleteFilesAsync(publicIds);
        }
        catch (Exception e)
        {
            await _unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }

        return Result.Success(new Success(MediaMessage.DeleteFilesSuccessfully.GetMessage().Message,
            MediaMessage.DeleteFilesSuccessfully.GetMessage().Code));
    }
}
