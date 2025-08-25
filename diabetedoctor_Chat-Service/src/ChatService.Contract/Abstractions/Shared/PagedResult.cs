namespace ChatService.Contract.Abstractions.Shared;

public class PagedResult<T>
{
    public List<T> Items { get; }
    public int PageIndex { get; }
    public int PageSize { get; }
    public long TotalCount { get; }
    public long TotalPages => (TotalCount / PageSize) + (TotalCount % PageSize > 0 ? 1 : 0);
    public bool HasNextPage => PageIndex * PageSize < TotalCount;
    public bool HasPreviousPage => PageIndex > 1;
 
    private PagedResult(List<T> items, int pageIndex, int pageSize, long totalCount)
    {
        Items = items;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
    
    public static PagedResult<T> Create(List<T> items, int pageIndex, int pageSize, long totalCount)
        => new(items, pageIndex, pageSize, totalCount);
    
    public static PagedResult<T> CreateEmpty(int pageIndex, int pageSize)
        => new([], pageIndex, pageSize, 0);
}
