namespace UserService.Contract.Common.Filters;

public record QueryFilter(string? Cursor, int PageSize = 1, bool IsSortAsc = false);