using ConsultationService.Domain.Models;
using MongoDB.Driver;

namespace ConsultationService.Domain.Abstractions;

public interface IMongoDbContext
{
    IMongoDatabase Database { get; }
    MongoClient Client { get; }
    
    IMongoCollection<User> Users { get; }
    IMongoCollection<Media> Medias { get; }
    IMongoCollection<ConsultationTemplate> ConsultationTemplates { get; }
    IMongoCollection<Consultation> Consultations { get; }
    IMongoCollection<Hospital> Hospitals { get; }
    IMongoCollection<OutboxEvent> OutboxEvents { get; }
    IMongoCollection<OutboxScheduleEvent> OutboxScheduleEvents { get; }
    IMongoCollection<OutboxEventConsumer> OutboxEventConsumers { get; }
    
}