using MediaService.Contract.Services.FavouriteCategory.Responses;

namespace MediaService.Contract.Services.FavouriteCategory;
public record GetAllPostsFromFavouriteCategoryQuery(string UserId) : IQuery<Success<IEnumerable<FavouriteCategoryResponse>>>;