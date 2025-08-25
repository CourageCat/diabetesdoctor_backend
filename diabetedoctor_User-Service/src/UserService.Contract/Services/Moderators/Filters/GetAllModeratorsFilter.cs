namespace UserService.Contract.Services.Moderators.Filters;

public record GetAllModeratorsFilter
{
    public string? Search { get; init; }
    public GenderEnum? Gender { get; init; }
    public string SortBy { get; init; } = string.Empty;
    public SortDirectionEnum SortDirection { get; init; } = SortDirectionEnum.Asc;
}