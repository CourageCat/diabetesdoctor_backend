namespace MediaService.Contract.Abstractions.Shared;

public class PagedList<T>
{
    public List<T> Items { get; set; }
    public int PageSize { get; init; }
    public string NextCursor { get; init; }
    public bool HasNextPage { get; init; }

    private PagedList(List<T> items, int pageSize, string nextCursor, bool hasNextPage)
    {
        Items = items;
        PageSize = pageSize;
        NextCursor = nextCursor;
        HasNextPage = hasNextPage;
    }

    public static PagedList<T> Create(List<T> items, int pageSize, string nextCursor, bool hasNext)
        => new(items, pageSize, nextCursor, hasNext);


    //public static async Task<PagedResult<T>> CreateAsync(IQueryable<T> query, int pageIndex, int pageSize)
    //{
    //    pageIndex = pageIndex <= 0 ? DefaultPageIndex : pageIndex;
    //    pageSize = pageSize <= 0
    //        ? DefaultPageSize
    //        : pageSize > UpperPageSize
    //        ? UpperPageSize : pageSize;

    //    var totalCount = await query.CountAsync();
    //    var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
    //    return new(items, pageIndex, pageSize, totalCount);
    //}

    //public static PagedResult<T> Create(List<T> items, int pageIndex, int pageSize, int totalCount)
    //    => new(items, pageIndex, pageSize, totalCount);
}