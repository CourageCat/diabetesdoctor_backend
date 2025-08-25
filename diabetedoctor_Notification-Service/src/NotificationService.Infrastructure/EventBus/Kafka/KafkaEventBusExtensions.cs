using NotificationService.Contract.EventBus.Abstractions;
using NotificationService.Contract.Settings;
using NotificationService.Infrastructure.Options;
namespace NotificationService.Infrastructure.EventBus.Kafka;

public static class KafkaEventBusExtensions
{
    public static void AddKafkaProducer(this IHostApplicationBuilder builder, KafkaSettings kafkaSettings,
        string connectionName = "kafka")
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        builder.AddKafkaProducer<string, EventEnvelope>(connectionName,
            configureSettings: (settings) =>
            {
                settings.Config.BootstrapServers = kafkaSettings.BootstrapServer;
                settings.Config.Acks = Acks.All;
                settings.Config.MessageSendMaxRetries = 3;
                settings.Config.CompressionType = CompressionType.Gzip;
                settings.Config.MessageTimeoutMs = 10000;
                settings.Config.RequestTimeoutMs = 10000;
                settings.Config.RetryBackoffMs = 1000;
                settings.Config.SaslUsername = kafkaSettings.SaslUsername;
                settings.Config.SaslPassword = kafkaSettings.SaslPassword;
                settings.Config.SecurityProtocol = SecurityProtocol.SaslPlaintext;
                settings.Config.SaslMechanism = SaslMechanism.Plain;
            },
            configureBuilder: (producerBuilder) => { producerBuilder.SetValueSerializer(new EventEnvelopeSerializer()); }
        );
    }

    public static void AddKafkaEventPublisher(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTransient<IEventPublisher>(services => new KafkaEventPublisher(
            services.GetRequiredService<IProducer<string, EventEnvelope>>(),
            services.GetRequiredService<ILogger<KafkaEventPublisher>>()
        ));
    }

    private static void AddKafkaMessageEnvelopConsumer(this IHostApplicationBuilder builder,
        KafkaSettings kafkaSettings,
        string groupId, string connectionName = "kafka")
    {
        builder.AddKeyedKafkaConsumer<string, EventEnvelope>(connectionName, configureSettings: (settings) =>
        {
            settings.Config.BootstrapServers = kafkaSettings.BootstrapServer;
            settings.Config.GroupId = groupId;
            settings.Config.AutoOffsetReset = AutoOffsetReset.Earliest;
            settings.Config.EnableAutoCommit = false;
            settings.Config.SaslMechanism = SaslMechanism.Plain;
            settings.Config.SecurityProtocol = SecurityProtocol.SaslPlaintext;
            settings.Config.SaslUsername = kafkaSettings.SaslUsername;
            settings.Config.SaslPassword = kafkaSettings.SaslPassword;
        },
            configureBuilder: (consumerBuilder) =>
            {
                consumerBuilder.SetValueDeserializer(new EventEnvelopeDeserializer());
            }
        );
    }

    public static void AddKafkaEventConsumer<T>(this IHostApplicationBuilder builder, KafkaSettings kafkaSettings,
        Action<ConsumerWorkerOptions>? configureOptions = null, string connectionName = "kafka") where T : class
    {
        var options = new ConsumerWorkerOptions();
        configureOptions?.Invoke(options);

        builder.AddKafkaMessageEnvelopConsumer(kafkaSettings, options.KafkaGroupId, connectionName: connectionName);
        // builder.Services.AddSingleton(options);
        // builder.Services.AddSingleton(services => options.IntegrationEventFactory);
        builder.Services.AddHostedService<KafkaSubscriberBase<T>>(sp =>
        {
            var consumer = sp.GetRequiredKeyedService<IConsumer<string, EventEnvelope>>(connectionName);
            var logger = sp.GetRequiredService<ILogger<KafkaSubscriberBase<T>>>();
            var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            var integrationEventFactory = options.IntegrationEventFactory;
            return new KafkaSubscriberBase<T>(consumer, options, logger, scopeFactory, integrationEventFactory, kafkaSettings);
        });
    }


    public static bool IsEvent<T1>(this IntegrationEvent @event)
    {
        return @event.GetType() == typeof(T1);
    }

    public static bool IsEvent<T1, T2>(this IntegrationEvent @event)
    {
        return @event.GetType() == typeof(T1) || @event.GetType() == typeof(T2);
    }

    public static bool IsEvent<T1, T2, T3>(this IntegrationEvent @event)
    {
        return @event.GetType() == typeof(T1) || @event.GetType() == typeof(T2) || @event.GetType() == typeof(T3);
    }

    public static bool IsEvent<T1, T2, T3, T4>(this IntegrationEvent @event)
    {
        return @event.GetType() == typeof(T1) || @event.GetType() == typeof(T2) || @event.GetType() == typeof(T3) ||
               @event.GetType() == typeof(T4);
    }
    
    public static bool IsEvent<T1, T2, T3, T4, T5>(this IntegrationEvent @event)
    {
        return @event.GetType() == typeof(T1) || @event.GetType() == typeof(T2) || @event.GetType() == typeof(T3) ||
               @event.GetType() == typeof(T4) || @event.GetType() == typeof(T5);
    }

    private class EventEnvelopeSerializer : ISerializer<EventEnvelope>
    {
        public byte[] Serialize(EventEnvelope data, SerializationContext context)
        {
            return JsonSerializer.SerializeToUtf8Bytes(data);
        }
    }

    private class EventEnvelopeDeserializer : IDeserializer<EventEnvelope>
    {
        public EventEnvelope Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonSerializer.Deserialize<EventEnvelope>(data) ?? throw new Exception("Error deserialize data");
        }
    }
}