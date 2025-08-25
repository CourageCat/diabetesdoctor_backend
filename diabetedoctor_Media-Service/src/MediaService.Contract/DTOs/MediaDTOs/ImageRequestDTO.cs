using MongoDB.Bson.Serialization.Attributes;

namespace MediaService.Contract.DTOs.MediaDTOs;

public class UploadImagesRequestDTO
{
    public UploadImagesRequestDTO(IFormFileCollection  images)
    {
        Images = images;
    }

    [BsonElement("images")]
    public IFormFileCollection Images { get; init; }
}
