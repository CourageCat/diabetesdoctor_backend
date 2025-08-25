using Elastic.Clients.Elasticsearch.MachineLearning;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotificationService.Contract.Settings;
using NotificationService.Domain;
using NotificationService.Domain.Abstractions;
using NotificationService.Domain.Models;

namespace NotificationService.Persistence;

public class MongoDbContext : IMongoDbContext
{
    public MongoDbContext(IOptions<MongoDbSettings> mongoDbSetting)
    {
        Client = new MongoClient(mongoDbSetting.Value.ConnectionString);
        Database = Client.GetDatabase(mongoDbSetting.Value.DatabaseName);
    }

    public IMongoDatabase Database { get; }
    public IMongoClient Client { get; }

    public IMongoCollection<User> Users => Database.GetCollection<User>(nameof(User));
    public IMongoCollection<Notification> Notifications => Database.GetCollection<Notification>(nameof(Notification));
    public IMongoCollection<OutboxEvent> OutboxEvents => Database.GetCollection<OutboxEvent>(nameof(OutboxEvent));
    public IMongoCollection<OutboxEventConsumer> OutboxEventConsumers => Database.GetCollection<OutboxEventConsumer>(nameof(OutboxEventConsumer));
}