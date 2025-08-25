namespace ConsultationService.Contract.Common.Pagination;

public record CursorPaginationRequest(string? Cursor, int PageSize = 20);