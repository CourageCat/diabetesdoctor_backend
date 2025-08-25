using MediaService.Contract.Services.Post;
using static MediaService.Contract.Services.Post.Filter;

namespace MediaService.Contract.Services.Like;
public record GetAllLikePostsQuery(string UserId) : IQuery<Success<IEnumerable<PostResponse>>>;
