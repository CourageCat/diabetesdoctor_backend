using MediaService.Domain.Abstractions;
using MediaService.Domain.Models;

namespace MediaService.Persistence;

public class MongoDbContext : IMongoDbContext
{
    public MongoDbContext(IOptions<MongoDbSetting> mongoDbSetting)
    {
        Client = new MongoClient(mongoDbSetting.Value.ConnectionString);
        Database = Client.GetDatabase(mongoDbSetting.Value.DatabaseName);
    }

    public IMongoDatabase Database { get; }
    public IMongoClient Client { get; }
    
    public IMongoCollection<Post> Posts => Database.GetCollection<Post>(nameof(Post));
    public IMongoCollection<User> Users => Database.GetCollection<User>(nameof(User));
    public IMongoCollection<Category> Categories => Database.GetCollection<Category>(nameof(Category));
    public IMongoCollection<PostCategory> PostCategories => Database.GetCollection<PostCategory>(nameof(PostCategory));
    public IMongoCollection<FavouriteCategory> FavouriteCategories => Database.GetCollection<FavouriteCategory>(nameof(FavouriteCategory));
    public IMongoCollection<BookMark> BookMarks => Database.GetCollection<BookMark>(nameof(BookMark));

    public IMongoCollection<Media> Medias => Database.GetCollection<Media>(nameof(Media));
    public IMongoCollection<Like> Likes => Database.GetCollection<Like>(nameof(Like));
    public IMongoCollection<OutboxEvent> OutboxEvents => Database.GetCollection<OutboxEvent>(nameof(OutboxEvent));
    public IMongoCollection<OutboxEventConsumer> OutboxEventConsumers => Database.GetCollection<OutboxEventConsumer>(nameof(OutboxEventConsumer));
}