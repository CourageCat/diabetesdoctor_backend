using ConsultationService.Contract.Settings;
using ConsultationService.Domain.Abstractions;
using ConsultationService.Domain.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ConsultationService.Persistence;

public class MongoDbContext : IMongoDbContext
{
    public MongoDbContext(IOptions<MongoDbSettings> mongoDbSetting)
    {
        Client = new MongoClient(mongoDbSetting.Value.ConnectionString);
        Database = Client.GetDatabase(mongoDbSetting.Value.DatabaseName);
    }

    public IMongoDatabase Database { get; }
    public MongoClient Client { get; }    
    public IMongoCollection<User> Users => Database.GetCollection<User>(nameof(User));    
    public IMongoCollection<Media> Medias => Database.GetCollection<Media>(nameof(Media));
    public IMongoCollection<ConsultationTemplate> ConsultationTemplates => Database.GetCollection<ConsultationTemplate>(nameof(ConsultationTemplate));
    public IMongoCollection<Consultation> Consultations => Database.GetCollection<Consultation>(nameof(Consultation));
    public IMongoCollection<Hospital> Hospitals => Database.GetCollection<Hospital>(nameof(Hospital));
    public IMongoCollection<OutboxEvent> OutboxEvents => Database.GetCollection<OutboxEvent>(nameof(OutboxEvent));
    public IMongoCollection<OutboxScheduleEvent> OutboxScheduleEvents => Database.GetCollection<OutboxScheduleEvent>(nameof(OutboxScheduleEvent));
    public IMongoCollection<OutboxEventConsumer> OutboxEventConsumers => Database.GetCollection<OutboxEventConsumer>(nameof(OutboxEventConsumer));
}