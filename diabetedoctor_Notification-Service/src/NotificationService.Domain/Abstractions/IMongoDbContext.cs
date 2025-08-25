using MongoDB.Driver;
using NotificationService.Domain.Models;

namespace NotificationService.Domain.Abstractions
{
    public interface IMongoDbContext 
    {
        public IMongoDatabase Database { get; }
        public IMongoClient Client { get; }

        public IMongoCollection<Models.Notification> Notifications { get; }
        public IMongoCollection<Models.User> Users { get; }
        public IMongoCollection<OutboxEvent> OutboxEvents { get; }
        public IMongoCollection<OutboxEventConsumer> OutboxEventConsumers { get; }
    }
}
