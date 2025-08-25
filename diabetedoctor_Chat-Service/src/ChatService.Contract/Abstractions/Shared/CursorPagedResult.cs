namespace ChatService.Contract.Abstractions.Shared;

public class CursorPagedResult<T> 
{
    public List<T> Items { get; set; }
    public long TotalItems { get; init; }
    public int PageSize { get; init; }
    public string NextCursor { get; init; }
    public bool HasNextPage { get; init; }

    private CursorPagedResult(List<T> items, long total, int pageSize, string nextCursor, bool hasNextPage)
    {
        Items = items;
        TotalItems = total;
        PageSize = pageSize;
        NextCursor = nextCursor;
        HasNextPage = hasNextPage;
    }
    
    public static CursorPagedResult<T> Create(List<T> items, long total, int pageSize, string nextCursor, bool hasNext)
        => new(items, total, pageSize, nextCursor, hasNext);
    
}