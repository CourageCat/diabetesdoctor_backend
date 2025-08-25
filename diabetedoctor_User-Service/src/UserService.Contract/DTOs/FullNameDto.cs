namespace UserService.Contract.DTOs;

public record FullNameDto
{
    public string FirstName { get; init; } = null!;
    public string? MiddleName { get; init; } = null!;
    public string LastName { get; init; } = null!;
}