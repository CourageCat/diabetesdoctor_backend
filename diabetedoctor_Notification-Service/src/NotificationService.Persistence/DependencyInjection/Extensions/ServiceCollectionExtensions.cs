using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService.Contract.Settings;
using NotificationService.Domain;
using NotificationService.Domain.Abstractions;
using NotificationService.Domain.Abstractions.Repositories;
using NotificationService.Persistence.Repositories;

namespace NotificationService.Persistence.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        
        builder.AddDatabaseConfiguration();
        builder.AddConfigurationService(builder.Configuration);
        
        builder.Services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IConversationRepository, ConversationRepository>()
            .AddScoped<INotificationRepository, NotificationRepository>()
            .AddScoped<IOutboxEventRepository, OutboxEventRepository>()
            .AddScoped<IOutboxEventConsumerRepository, OutboxEventConsumerRepository>();
    }

    private static void AddConfigurationService(this IHostApplicationBuilder builder, IConfiguration configuration)
    {
        builder.Services
            .Configure<MongoDbSettings>(configuration.GetSection(MongoDbSettings.SectionName));
    }

    private static void AddDatabaseConfiguration(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IMongoDbContext, MongoDbContext>();
    }
}
