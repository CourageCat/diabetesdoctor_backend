using MongoDB.Bson.Serialization.Attributes;

namespace MediaService.Contract.DTOs.MediaDTOs;

public record ImageDto
{
    public ImageDto(string imageUrl, string publicImageId)
    {
        ImageUrl = imageUrl;
        PublicImageId = publicImageId;
    }

    [BsonElement("image_url")]
    public string ImageUrl { get; init; }
    [BsonElement("public_image_id")]
    public string PublicImageId { get; init; }
}