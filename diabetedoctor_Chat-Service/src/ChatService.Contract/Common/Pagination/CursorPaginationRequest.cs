using System.ComponentModel;

namespace ChatService.Contract.Common.Pagination;

public record CursorPaginationRequest(string? Cursor, int PageSize = 20);