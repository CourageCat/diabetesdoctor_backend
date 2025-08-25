using MongoDB.Bson;

namespace MediaService.Contract.Services.Like;

public record LikePostCommand(string UserId, ObjectId PostId) : ICommand<Success>;

