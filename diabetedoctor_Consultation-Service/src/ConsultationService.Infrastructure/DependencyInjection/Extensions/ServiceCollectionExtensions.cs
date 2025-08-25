using ConsultationService.Contract.EventBus.Abstractions;
using ConsultationService.Contract.EventBus.Abstractions.Message;
using ConsultationService.Contract.EventBus.Events.ConsultationIntegrationEvents;
using ConsultationService.Contract.EventBus.Events.ConversationIntegrationEvents;
using ConsultationService.Contract.EventBus.Events.UserIntegrationEvents;
using ConsultationService.Contract.Infrastructure.Services;
using ConsultationService.Contract.Settings;
using ConsultationService.Infrastructure.EventBus;
using ConsultationService.Infrastructure.EventBus.Ably;
using ConsultationService.Infrastructure.EventBus.Kafka;
using ConsultationService.Infrastructure.EventBus.Kafka.EventSubscribers;
using ConsultationService.Infrastructure.Idempotence;
using ConsultationService.Infrastructure.Outbox;
using ConsultationService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsultationService.Infrastructure.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureService(this IHostApplicationBuilder builder)
    {
        builder.AddKafka();
        builder.AddOutboxService();
        builder.Services.AddSingleton<IIntegrationEventFactory, IntegrationEventFactory>();
        builder.Services
            .AddScoped<IClaimsService, ClaimsService>()
            .AddScoped<ICloudinaryService, CloudinaryService>();
        
        builder.Services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(classes => classes.AssignableTo(typeof(IIntegrationEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        builder.Services.Decorate(typeof(IIntegrationEventHandler<>), typeof(IdempotenceIntegrationEventHandler<>));
    }

    private static void AddKafka(this IHostApplicationBuilder builder)
    {
        var kafkaSettings = builder.Configuration
            .GetSection(KafkaSettings.SectionName)
            .Get<KafkaSettings>() ?? throw new InvalidOperationException("Kafka config missing");
        
        builder.AddKafkaProducer(kafkaSettings);
        builder.AddKafkaEventPublisher();
        builder.AddKafkaEventConsumer<UserSubscriber>(kafkaSettings, configureOptions: options =>
        {
            options.KafkaGroupId = kafkaSettings.UserTopicConsumerGroup;
            options.Topic = kafkaSettings.UserTopic;
            options.IntegrationEventFactory = IntegrationEventFactory.Instance;
            options.ServiceName = nameof(ConsultationService);
            options.AcceptEvent = e => e.IsEvent<UserInfoCreatedProfileIntegrationEvent>();
        }, connectionName: kafkaSettings.UserConnectionName);
        
        builder.AddKafkaEventConsumer<ConsultationSubscriber>(kafkaSettings, configureOptions: options =>
        {
            options.KafkaGroupId = kafkaSettings.ConsultationTopicConsumerGroup;
            options.Topic = kafkaSettings.ConsultationTopic;
            options.IntegrationEventFactory = IntegrationEventFactory.Instance;
            options.ServiceName = nameof(ConsultationService);
            options.AcceptEvent = e => e.IsEvent<ConsultationStartedIntegrationEvent, ConsultationEndedIntegrationEvent>();
        }, connectionName: kafkaSettings.ConsultationConnectionName);
        
        builder.AddKafkaEventConsumer<ConversationSubscriber>(kafkaSettings, configureOptions: options =>
        {
            options.KafkaGroupId = kafkaSettings.ConversationTopicConsumerGroup;
            options.Topic = kafkaSettings.ConversationTopic;
            options.IntegrationEventFactory = IntegrationEventFactory.Instance;
            options.ServiceName = nameof(ConsultationService);
            options.AcceptEvent = e => e.IsEvent<ConversationCreatedIntegrationEvent, ConversationLinkedIntegrationEvent>();
        }, connectionName: kafkaSettings.ConversationConnectionName);
        
        builder.AddKafkaEventConsumer<RetrySubscriber>(kafkaSettings, options =>
        {
            options.KafkaGroupId = kafkaSettings.RetryTopicConsumerGroup;
            options.Topic = kafkaSettings.RetryTopic;
            options.IntegrationEventFactory = IntegrationEventFactory.Instance;
            options.ServiceName = nameof(ConsultationService);
        }, kafkaSettings.RetryConnectionName);
    }

    private static void AddAbly(this IHostApplicationBuilder builder)
    {
        builder.AddAblyRealtime();
        builder.AddAblyEventPublisher();
        // builder.AddAblySubscriber();
    }
    
    private static void AddOutboxService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<OutboxProcessor>();
        builder.AddOutboxBackgroundService();
        builder.AddOutboxRetryBackgroundService(opt => opt.RetryCount = 1);
        // builder.AddOutboxRetryBackgroundService(opt => opt.RetryCount = 2);
    }
    

}