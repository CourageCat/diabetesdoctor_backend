namespace UserService.Contract.Common.Pagination;

public record OffsetPaginationRequest(int PageIndex = 0, int PageSize = 20);