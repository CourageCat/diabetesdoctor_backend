namespace AuthService.Api.Persistences.Data.ValueObjects;

public class Image : ValueObject
{
    public string PublicId { get; }
    public string Url { get; }

    private Image() { }

    public Image(string publicId, string url)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            throw new ArgumentException("PublicId is required.");

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Invalid avatar URL.");

        PublicId = publicId;
        Url = url;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PublicId;
        yield return Url;
    }

    public override string ToString() => Url;
}

