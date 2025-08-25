namespace UserService.Contract.Services.Hospitals.Responses;

public record HospitalResponse
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string PhoneNumber { get; init; } = null!;
    public string Website { get; init; } = null!;
    public string Address { get; init; } = null!;
    public string Introduction { get; init; } = null!;
    public string Thumbnail { get; init; } = null!;
    public IEnumerable<ImageResponseDto> Images { get; init; } = null!;
    public DateTime? CreatedDate  { get; init; }
}