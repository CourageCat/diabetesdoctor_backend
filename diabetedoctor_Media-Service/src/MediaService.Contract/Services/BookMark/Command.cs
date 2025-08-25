using MongoDB.Bson;

namespace MediaService.Contract.Services.BookMark;

public record UpdateBookMarkCommand(ObjectId PostId, string UserId) : ICommand<Success>;
public record RemoveAllPostsFromBookMarkCommand(string UserId) : ICommand<Success>;

