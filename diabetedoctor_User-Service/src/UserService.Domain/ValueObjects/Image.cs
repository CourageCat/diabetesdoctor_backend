namespace UserService.Domain.ValueObjects;

public sealed class Image : ValueObject
{
    public string PublicId { get; }
    public string Url { get; }

    private Image() { }

    private Image(string publicId, string url)
    {
        PublicId = publicId;
        Url = url;
    }

    public static Image Of(string publicId, string url)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            throw new ArgumentException("PublicId bắt buộc");
        
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            throw new ArgumentException("Url không phù hợp");

        return new Image(publicId.Trim(), url.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PublicId;
        yield return Url;
    }

    public override string ToString() => Url;
}

