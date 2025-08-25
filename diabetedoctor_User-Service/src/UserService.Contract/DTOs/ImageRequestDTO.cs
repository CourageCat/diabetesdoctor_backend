using Microsoft.AspNetCore.Http;

namespace UserService.Contract.DTOs;

public record UploadImagesRequestDTO
{
    public UploadImagesRequestDTO(IFormFileCollection images)
    {
        Images = images;
    }
    
    public IFormFileCollection Images { get; init; }
}
