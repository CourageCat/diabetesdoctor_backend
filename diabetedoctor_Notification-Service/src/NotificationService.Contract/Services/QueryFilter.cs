namespace NotificationService.Contract.Services;

public class QueryFilter
{
    public string? Cursor { get; set; } = default;
    public int? PageSize { get; set; } = default;
}