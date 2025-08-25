namespace UserService.Contract.Common.Filters;

public record QueryPageIndex(int PageIndex = 1, int PageSize = 10, bool IsSortAsc = false);