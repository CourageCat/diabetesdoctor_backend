using Microsoft.EntityFrameworkCore;

namespace UserService.Contract.Abstractions.Shared;

public class OffsetPagedResult<T>
{
    private const int DefaultPageIndex = 1;
    private const int DefaultPageSize = 20;
    private const int UpperPageSize = 1000;
    
    public List<T> Items { get; }
    public int PageIndex { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => (TotalCount / PageSize) + (TotalCount % PageSize > 0 ? 1 : 0);
    public bool HasNextPage => PageIndex * PageSize < TotalCount;
    public bool HasPreviousPage => PageIndex > 1;
    
    private OffsetPagedResult(List<T> items, int pageIndex, int pageSize, int totalCount)
    {
        Items = items;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
    
    public static OffsetPagedResult<T> Create(List<T> items, int pageIndex, int pageSize, int totalCount)
        => new(items, pageIndex, pageSize, totalCount);
    
    public static OffsetPagedResult<T> CreateEmpty(int pageIndex, int pageSize)
        => new([], pageIndex, pageSize, 0);

    public static async Task<OffsetPagedResult<T>> CreateAsync(IQueryable<T> query, int pageIndex, int pageSize, int totalCount)
    {
        pageIndex = pageIndex > 0 ? pageIndex : DefaultPageIndex;
        pageSize = pageSize > 0
            ? pageSize > UpperPageSize ? UpperPageSize : pageSize
            : DefaultPageSize;
        
        var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new OffsetPagedResult<T>(items, pageIndex, pageSize, totalCount);
    }

    //public static PagedResult<T> Create(List<T> items, int pageIndex, int pageSize, int totalCount)
    //    => new(items, pageIndex, pageSize, totalCount);
}
