using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Hospitals.Queries;
using UserService.Contract.Services.Hospitals.Responses;

namespace UserService.Application.UseCases.V1.Queries.Hospitals;

public sealed class GetHospitalByIdQueryHandler(ApplicationDbContext context) : IQueryHandler<GetHospitalByIdQuery, Success<HospitalResponse>>
{
    public async Task<Result<Success<HospitalResponse>>> Handle(GetHospitalByIdQuery query, CancellationToken cancellationToken)
    {
        var hospitalFound = await context.HospitalProfiles.Include(hospital => hospital.Medias)
            .FirstOrDefaultAsync(hospital => hospital.Id == query.HospitalId, cancellationToken: cancellationToken);
        if (hospitalFound is null)
        {
            return FailureFromMessage(HospitalErrors.HospitalNotFound);
        }

        var images = new List<ImageResponseDto>(); 
        hospitalFound.Medias.ToList().ForEach(media =>
        {
            if (media.IsUsed)
            {
                var imageResponseDto = new ImageResponseDto
                {
                    Id = media.Id.ToString(),
                    ImageUrl = media.PublicUrl
                };
                images.Add(imageResponseDto);
            }
        });
        // Remove thumbnail in Images of Hospital
        for (var i = 0; i < images.Count; i++)
        {
            if (images[i].ImageUrl == hospitalFound.Thumbnail.Url)
            {
                images.Remove(images[i]);
                break;
            }
        }
        
        var result = new HospitalResponse()
        {
            Id = hospitalFound.Id.ToString(),
            Name = hospitalFound.Name,
            Email = hospitalFound.Email,
            PhoneNumber = hospitalFound.PhoneNumber,
            Website =  hospitalFound.Website,
            Address = hospitalFound.Address,
            Introduction = hospitalFound.Introduction,
            Thumbnail = hospitalFound.Thumbnail.Url,
            CreatedDate = hospitalFound.CreatedDate,
            Images = images
        };
        
        return Result.Success(new Success<HospitalResponse>(
            HospitalMessages.GetHospitalByIdSuccessfully.GetMessage().Code,
            HospitalMessages.GetHospitalByIdSuccessfully.GetMessage().Message,
            result));
    }
    
    private static Result<Success<HospitalResponse>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<HospitalResponse>>(error);
    }
}