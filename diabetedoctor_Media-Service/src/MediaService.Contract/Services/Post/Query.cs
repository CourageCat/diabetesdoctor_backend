using MediaService.Contract.Enumarations.Post;
using MediaService.Contract.Services.BookMark;
using MongoDB.Bson;
using static MediaService.Contract.Services.Post.Filter;

namespace MediaService.Contract.Services.Post;
public record GetAllPostsQuery(string? Cursor, int PageSize, PostFilter FilterParams, string SortType, bool IsSortAsc) : IQuery<Success<PagedList<PostResponse>>>;
public record GetAllPostsBySystemQuery(int PageIndex, int PageSize, PostFilter FilterParams, string SortType, bool IsSortAsc) : IQuery<Success<PagedResult<PostResponse>>>;
public record GetPostByIdQuery(ObjectId Id, string? UserId) : IQuery<Success<PostResponse>>;
public record GetPostByIdBySystemQuery(ObjectId Id) : IQuery<Success<PostResponse>>;
public record GetTopViewPostsQuery(string? UserId, int NumberOfPosts, int NumberOfDays) : IQuery<Success<IEnumerable<PostResponse>>>;
public record GetAllLikePostsQuery(string? Cursor, int PageSize, PostFilter FilterParams, string SortType, bool IsSortAsc) : IQuery<Success<PagedList<PostResponse>>>;
public record GetAllPostsFromBookMarkQuery(string? Cursor, int PageSize, PostFilter FilterParams, string SortType, bool IsSortAsc) : IQuery<Success<PagedList<PostResponse>>>;



