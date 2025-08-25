using MediaService.Domain.Models;
using MongoDB.Driver;

namespace MediaService.Domain.Abstractions;

public interface IMongoDbContext
{
    public IMongoDatabase Database { get; }
    public IMongoClient Client { get; }
    
    public IMongoCollection<Post> Posts { get; }
    public IMongoCollection<User> Users { get; }
    public IMongoCollection<Category> Categories { get; }
    public IMongoCollection<PostCategory> PostCategories { get; }
    public IMongoCollection<FavouriteCategory> FavouriteCategories { get; }
    public IMongoCollection<BookMark> BookMarks { get; }
    public IMongoCollection<Media> Medias { get; }
    public IMongoCollection<Like> Likes { get; }
    public IMongoCollection<OutboxEvent> OutboxEvents { get; }
    public IMongoCollection<OutboxEventConsumer> OutboxEventConsumers { get; }
}