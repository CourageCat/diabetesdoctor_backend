using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Hospitals.Commands;

namespace UserService.Application.UseCases.V1.Commands.Hospitals;

public sealed class UpdateHospitalCommandHandler : ICommandHandler<UpdateHospitalCommand, Success>
{
    private readonly IRepositoryBase<HospitalProfile, Guid> _hospitalRepository;
    private readonly IRepositoryBase<Media, Guid> _mediaRepository;

    public UpdateHospitalCommandHandler(IRepositoryBase<HospitalProfile, Guid> hospitalRepository, IRepositoryBase<Media, Guid> mediaRepository)
    {
        _hospitalRepository = hospitalRepository;
        _mediaRepository = mediaRepository;
    }

    public async Task<Result<Success>> Handle(UpdateHospitalCommand command, CancellationToken cancellationToken)
    {
        var hospitalFound = await _hospitalRepository.FindSingleAsync(x => x.Id == command.Id, true, cancellationToken, includeProperties: p => p.Medias);
        if (hospitalFound is null)
        {
            return FailureFromMessage(HospitalErrors.HospitalNotFound);
        }
        var requestImages = command.Images;
        var hospitalImages = new List<Guid>();
        hospitalFound.Medias.ToList().ForEach(image =>
        {
            if (image.IsUsed)
            {
                hospitalImages.Add(image.Id);
            }
        });

        Image? thumbnail = null;
        if (command.Thumbnail != null)
        {
            requestImages.Add(command.Thumbnail.Value);
            var thumbnailFound = await _mediaRepository.FindSingleAsync(x => x.Id == command.Thumbnail.Value, true, cancellationToken);
            if (thumbnailFound is null)
            {
                return FailureFromMessage(MediaErrors.ImageNotFound);
            }
            thumbnail = Image.Of(thumbnailFound.PublicId, thumbnailFound.PublicUrl);
        }
        else
        {
            var oldThumbnail = await _mediaRepository.FindSingleAsync(m => m.PublicUrl == hospitalFound.Thumbnail.Url, true, cancellationToken);
            if (oldThumbnail is not null)
            {
                requestImages.Add(oldThumbnail.Id);
            }
        }
        // Handle unuse images which is not in request images
        var unuseImages = hospitalImages.Where(image => !requestImages.Contains(image)).ToList();
        if (unuseImages.Count > 0)
        {
            _mediaRepository.UpdateMany(media => unuseImages.Contains(media.Id), setters => setters
                .SetProperty(image => image.IsUsed, false)
                .SetProperty(image => image.ExpiredAt, DateTime.UtcNow.AddHours(1))
                .SetProperty(image => image.ModifiedDate, DateTime.UtcNow));
        }
        // Handle use new images (in request) which is not in old images
        var useImages = requestImages.Where(image => !hospitalImages.Contains(image)).ToList();
        if (useImages.Count > 0)
        {
            _mediaRepository.UpdateMany(media => useImages.Contains(media.Id), setters => setters
                .SetProperty(image => image.IsUsed, true)
                .SetProperty(image => image.ExpiredAt, (DateTime?)null)
                .SetProperty(image => image.ModifiedDate, DateTime.UtcNow)
                .SetProperty(image => image.HospitalProfileId, hospitalFound.Id));
        }
        hospitalFound.Update(command.Name, command.Email, command.PhoneNumber, command.Website, command.Address, command.Introduction, thumbnail);
        return Result.Success(new Success(HospitalMessages.UpdateHospitalSuccessfully.GetMessage().Code,
            HospitalMessages.UpdateHospitalSuccessfully.GetMessage().Message));
    }
    
    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}