using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MediaService.Contract.Services.Media;
public record MediaResponse
{
    public MediaResponse(string imageId, string publicId, string publicUrl)
    {
        ImageId = imageId;
        PublicId = publicId;
        PublicUrl = publicUrl;
    }

    [BsonElement("image_id"), BsonRepresentation(BsonType.ObjectId)]
    public string ImageId { get; init; }
    [BsonElement("public_id")]
    public string PublicId { get; init; }
    [BsonElement("public_url")]
    public string PublicUrl { get; init; }
}
