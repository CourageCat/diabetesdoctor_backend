using MediaService.Contract.Common.Constant.Event;
using MediaService.Contract.EventBus.Events.UserIntegrationEvents;
using MediaService.Domain.Models;
using MediaService.Infrastructure.EventBus.Kafka.EventSubscribers;
using MediaService.Infrastructure.Idempotence;
using MediaService.Infrastructure.Outbox;
using Microsoft.Extensions.Configuration;

namespace MediaService.Infrastructure.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureService(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddSingleton<INormalizeService, NormalizeService>()
            .AddSingleton<IMediaService, CloudinaryService>()
            .AddScoped<IUserContext, UserContext>()
            .AddSingleton<IIntegrationEventFactory, IntegrationEventFactory>();
        builder.AddKafka();
        builder.AddOutboxService();
        builder.Services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(classes => classes.AssignableTo(typeof(IIntegrationEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        builder.Services.Decorate(typeof(IIntegrationEventHandler<>), typeof(IdempotenceIntegrationEventHandler<>));
        builder.AddKafkaEventPublisher();
        
    }
    
    private static void AddKafka(this IHostApplicationBuilder builder)
    {
        var kafkaSettings = builder.Configuration
            .GetSection(KafkaSettings.SectionName)
            .Get<KafkaSettings>() ?? throw new InvalidOperationException("Kafka config missing");
        
        builder.AddKafkaProducer(kafkaSettings);
        builder.AddKafkaEventPublisher();
        
        builder.AddKafkaEventConsumer<UserSubscriber>(kafkaSettings, options =>    
        {
            options.KafkaGroupId = kafkaSettings.UserTopicConsumerGroup;
            options.Topic = kafkaSettings.UserTopic;
            options.IntegrationEventFactory = IntegrationEventFactory.Instance;
            options.ServiceName = nameof(MediaService);
            options.AcceptEvent = e => e.IsEvent<UserInfoCreatedProfileIntegrationEvent>() || e.IsEvent<UserInfoUpdatedProfileIntegrationEvent>();
        }, kafkaSettings.UserConnectionName );
    }

    private static void AddOutboxService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<OutboxProcessor>();
        builder.AddOutboxBackgroundService();
        builder.AddOutboxRetryBackgroundService(option => option.RetryCount = 1);
    }
}