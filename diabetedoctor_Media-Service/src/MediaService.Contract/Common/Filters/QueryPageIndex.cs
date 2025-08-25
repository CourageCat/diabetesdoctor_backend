namespace MediaService.Contract.Common.Filters;

public record QueryPageIndex
{
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
    public string? SortType { get; set; }
    public bool? IsSortAsc { get; set; }
}