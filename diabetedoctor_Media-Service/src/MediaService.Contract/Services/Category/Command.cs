using MongoDB.Bson;

namespace MediaService.Contract.Services.Category;

public record CreateCategoryCommand(string Name, string Description, string Image) : ICommand<Success>;
public record DeleteCategoryCommand(ObjectId Id) : ICommand<Success>;
public record UpdateCategoryCommand(ObjectId Id, string Name, string Description, ObjectId Image) : ICommand<Success>;
