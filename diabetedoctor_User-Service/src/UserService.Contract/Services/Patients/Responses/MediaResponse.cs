namespace UserService.Contract.Services.Patients.Responses;

public record MediaResponse
{
    public MediaResponse(string imageId, string publicId, string publicUrl)
    {
        ImageId = imageId;
        PublicId = publicId;
        PublicUrl = publicUrl;
    }
    public string ImageId { get; init; }
    public string PublicId { get; init; }
    public string PublicUrl { get; init; }
}