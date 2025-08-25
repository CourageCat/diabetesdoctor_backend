using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using NotificationService.Contract.EventBus.Abstractions;
using NotificationService.Contract.EventBus.Events.Conversations;
using NotificationService.Contract.EventBus.Events.Notifications;
using NotificationService.Contract.EventBus.Events.Users;
using NotificationService.Contract.Infrastructure;
using NotificationService.Contract.Settings;
using NotificationService.Infrastructure.EventBus;
using NotificationService.Infrastructure.EventBus.Kafka;
using NotificationService.Infrastructure.EventBus.Kafka.EventSubscribers;
using NotificationService.Infrastructure.Idempotence;
using NotificationService.Infrastructure.Outbox;
using NotificationService.Infrastructure.Services;

namespace NotificationService.Infrastructure.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureService(this IHostApplicationBuilder builder)
    {
        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pushnotifcm-7f80b-firebase-adminsdk-fbsvc-f8b3618c77.json"))
        });
        
        builder.AddKafka();
        builder.AddOutboxService();
        builder.Services.AddSingleton<IIntegrationEventFactory, IntegrationEventFactory>();
        builder.Services.AddSingleton(typeof(IFirebaseNotificationService<>), typeof(FirebaseNotificationService<>));
        builder.Services
            .AddScoped<IClaimsService, ClaimsService>();
        
            // .AddScoped<ICloudinaryService, CloudinaryService>();
        
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
        
        builder.AddKafkaEventConsumer<UserSubscriber>(kafkaSettings, options =>
        {
            options.KafkaGroupId = kafkaSettings.UserTopicConsumerGroup;
            options.Topic = kafkaSettings.UserTopic;
            options.IntegrationEventFactory = IntegrationEventFactory.Instance;
            options.ServiceName = nameof(NotificationService);
            options.AcceptEvent = e => e.IsEvent<UserInfoCreatedProfileIntegrationEvent, UserInfoUpdatedProfileIntegrationEvent, UserInfoFcmTokenUpdatedIntegrationEvent>();
        }, kafkaSettings.UserConnectionName);
        
        builder.AddKafkaEventConsumer<ChatSubscriber>(kafkaSettings, options =>
        {
            options.KafkaGroupId = kafkaSettings.ChatTopicConsumerGroup;
            options.Topic = kafkaSettings.ChatTopic;
            options.IntegrationEventFactory = IntegrationEventFactory.Instance;
            options.ServiceName = nameof(NotificationService);
            options.AcceptEvent = e => e.IsEvent<MessageCreatedIntegrationEvent>();
        }, kafkaSettings.ChatConnectionName);
        
        builder.AddKafkaEventConsumer<ConversationSubcriber>(kafkaSettings, options =>
        {
            options.KafkaGroupId = kafkaSettings.ConversationTopicConsumerGroup;
            options.Topic = kafkaSettings.ConversationTopic;
            options.IntegrationEventFactory = IntegrationEventFactory.Instance;
            options.ServiceName = nameof(NotificationService);
            options.AcceptEvent = e => e.IsEvent
            <ConversationCreatedIntegrationEvent, ConversationUpdatedIntegrationEvent, ConversationDeletedIntegrationEvent, 
                GroupMemberRemovedIntegrationEvent, GroupMembersAddedIntegrationEvent>();
        }, kafkaSettings.ConversationConnectionName);
        
        builder.AddKafkaEventConsumer<RetrySubscriber>(kafkaSettings, options =>
        {
            options.KafkaGroupId = kafkaSettings.RetryTopicConsumerGroup;
            options.Topic = kafkaSettings.RetryTopic;
            options.IntegrationEventFactory = IntegrationEventFactory.Instance;
            options.ServiceName = nameof(NotificationService);
        }, kafkaSettings.RetryConnectionName);
    }
    
    private static void AddOutboxService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<OutboxProcessor>();
        builder.AddOutboxBackgroundService();
        builder.AddOutboxRetryBackgroundService(opt => opt.RetryCount = 1);
        // builder.AddOutboxRetryBackgroundService(opt => opt.RetryCount = 2);
    }
    

}