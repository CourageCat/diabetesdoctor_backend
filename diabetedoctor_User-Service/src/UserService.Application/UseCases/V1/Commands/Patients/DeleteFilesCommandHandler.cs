using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Infrastructure;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Application.UseCases.V1.Commands.Patients;

public sealed class DeleteFilesCommandHandler : ICommandHandler<DeleteFilesCommand, Success>
{
    private readonly IMediaService _mediaService;
    private readonly IRepositoryBase<Domain.Models.Media, Guid> _mediaRepository;

    public DeleteFilesCommandHandler(IMediaService mediaService,
        IRepositoryBase<Domain.Models.Media, Guid> mediaRepository)
    {
        _mediaService = mediaService;
        _mediaRepository = mediaRepository;
    }

    public async Task<Result<Success>> Handle(DeleteFilesCommand command, CancellationToken cancellationToken)
    {
        var imagesFound =
            (await _mediaRepository.FindAllAsync(image => command.ImageIds.Contains(image.Id.ToString()),
                cancellationToken)).ToList();
        if (imagesFound.Count == 0)
        {
            return FailureFromMessage(MediaErrors.ImagesNotFound);
        }

        var publicIds = imagesFound.Select(x => x.PublicId).ToArray();

        _mediaRepository.RemoveMultiple(imagesFound);
        var resultDeletedImage = await _mediaService.DeleteFilesAsync(publicIds);

        return Result.Success(new Success(MediaMessages.DeleteFilesSuccessfully.GetMessage().Message,
            MediaMessages.DeleteFilesSuccessfully.GetMessage().Code));
    }
    
    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}