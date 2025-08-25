using MongoDB.Bson;

namespace MediaService.Contract.Services.FavouriteCategory;

public record UpdateFavouriteCategoryCommand(List<ObjectId> CategoryIds, string UserId) : ICommand<Success>;
