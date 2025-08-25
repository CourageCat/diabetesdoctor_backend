using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using UserService.Contract.EventBus.IntegrationEvents.Consultation;
using UserService.Contract.Infrastructure;
using UserService.Infrastructure.EventBus;
using UserService.Infrastructure.EventBus.Kafka;
using UserService.Infrastructure.EventBus.Kafka.EventSubscribers;
using UserService.Infrastructure.Idempotence;
using UserService.Infrastructure.Outbox;
using UserService.Infrastructure.Services;
using UserService.Infrastructure.Services.Gemini;

namespace UserService.Infrastructure.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureInfrastructureService(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddScoped<IUserContext, UserContext>()
            .AddSingleton<IIntegrationEventFactory, IntegrationEventFactory>();
        builder.AddKafka();
        builder.AddOutboxService();
        builder.Services.AddGemini(builder.Configuration);
        builder.Services.AddConfigurationRedis(builder.Configuration);
        builder.Services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(classes => classes.AssignableTo(typeof(IIntegrationEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        builder.Services.Decorate(typeof(IIntegrationEventHandler<>), typeof(IdempotenceIntegrationEventHandler<>));
        builder.AddKafkaEventPublisher();
        builder.Services.AddHostedService<QueuedHostedService>();

        builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        builder.Services.AddSingleton<IMediaService, CloudinaryService>();
        builder.Services.AddSingleton<IPaymentService, PayOSService>();
    }
    
    public static void AddConfigurationRedis
        (this IServiceCollection services, IConfiguration configuration)
    {
        var redisSettings = new RedisSettings();
        configuration.GetSection(RedisSettings.SectionName).Bind(redisSettings);
        services.AddSingleton<RedisSettings>();
        if (!redisSettings.Enabled) return;
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisSettings.ConnectionString));
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisSettings.ConnectionString;
        });
        services.AddSingleton<IResponseCacheService, ResponseCacheService>();
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
            options.KafkaGroupId = kafkaSettings.ConsultationTopicConsumerGroup;
            options.Topic = kafkaSettings.ConsultationTopic;
            options.IntegrationEventFactory = IntegrationEventFactory.Instance;
            options.ServiceName = nameof(UserService);
            options.AcceptEvent = e => e.IsEvent<ConsultationBookedIntegrationEvent, ConsultationCancelledIntegrationEvent, ConsultationEndedIntegrationEvent>();
        }, kafkaSettings.ConsultationConnectionName);
    }
    
    private static void AddGemini(this IServiceCollection services, IConfiguration configuration)
    {
        var geminiSettings = new GeminiSettings();
        configuration.GetSection(GeminiSettings.SectionName).Bind(geminiSettings);
        services.AddSingleton<GeminiSettings>();

        services.AddTransient<GeminiDelegatingHandler>();
        
        services.AddSingleton<IAiService, GeminiService>();
    }

    private static void AddOutboxService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<OutboxProcessor>();
        builder.AddOutboxBackgroundService();
        builder.AddOutboxRetryBackgroundService(option => option.RetryCount = 1);
    }
}