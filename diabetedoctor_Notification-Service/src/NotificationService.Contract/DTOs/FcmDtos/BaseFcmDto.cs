namespace NotificationService.Contract.DTOs.FcmDtos;

public abstract class BaseFcmDto
{
    public string Title { get; init; } = null!;
    public string? Body { get; init; }
    public string Icon { get; init; } = null!;

    public abstract Dictionary<string, string?> GetData();
}