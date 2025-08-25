using AuthService.Api.Infrastructures.Abstractions.EventsBus;
using AuthService.Api.Infrastructures.Abstractions.EventsBus.Message;
using AuthService.Api.Infrastructures.EventBus;
using AuthService.Api.Infrastructures.EventBus.Events;
using AuthService.Api.Infrastructures.EventBus.Kafka;
using AuthService.Api.Infrastructures.EventBus.Kafka.EventSubscribers;
using AuthService.Api.Infrastructures.idempotence;
using AuthService.Api.Infrastructures.Outbox;
using Twilio;

namespace AuthService.Api.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services
            .AddSwaggerGenNewtonsoftSupport()
            .AddFluentValidationRulesToSwagger()
            .AddEndpointsApiExplorer()
            .AddSwagger();

        return services;
    }

    public static void AddConfigurationAppSetting
        (this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<AuthSettings>(configuration.GetSection(AuthSettings.SectionName))
            .Configure<DefaultAvatarSettings>(configuration.GetSection(DefaultAvatarSettings.SectionName))
            .Configure<KafkaSettings>(configuration.GetSection(KafkaSettings.SectionName))
            .Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName))
            .Configure<TwilioSettings>(configuration.GetSection(TwilioSettings.SectionName));
    }

    private static IServiceCollection AddConfigureMediatR(this IServiceCollection services)
     => services.AddMediatR(config => config.RegisterServicesFromAssembly(AssemblyReference.Assembly))
        .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
        .AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>))
        .AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);

    private static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddSingleton<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        var connectionString = configuration["ConnectionStrings:DatabaseConnectionString"];
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });
    }

    private static void AddConfigurationRedis
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


    public static void ConfigureInfrastructureService(this IHostApplicationBuilder builder)
    {
        var twilioSettings = builder.Configuration
            .GetSection(TwilioSettings.SectionName)
            .Get<TwilioSettings>() ?? throw new InvalidOperationException("Twilio config missing");
        TwilioClient.Init(twilioSettings.AccountSid, twilioSettings.AuthToken);
        
        builder.Services.AddSingleton(typeof(IntegrationEventFactory));
        builder.Services.AddScoped<IIntegrationEventFactory, IntegrationEventFactory>();
        builder.Services.AddScoped<IClaimsService, ClaimsService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<ITwilioService, TwilioService>();

        builder.Services.AddConfigurationRedis(builder.Configuration);

        builder.Services
            .AddTransient<IPasswordHashService, PasswordHashService>()
            .AddTransient<IJwtProviderService, JwtProviderService>();
        
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

    public static void ConfigurePersistenceServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseConfiguration(builder.Configuration);

        builder.Services
            .AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork))
            .AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>))
            .AddScoped(typeof(IOutboxEventConsumerRepository), typeof(OutboxEventConsumerRepository));
    }

    public static void ConfigureApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddConfigureMediatR();
    }

    public static void ConfigureApiService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddConfigurationAppSetting(builder.Configuration);

        builder.Services.AddCarter();

        builder.Services
            .AddTransient<ExceptionHandlingMiddleware>()
            .AddTransient<RequireRoleMiddleware>();

        builder.Services.AddSwaggerServices();

        builder.Services
            .AddApiVersioning(options => options.ReportApiVersions = true)
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
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
            options.ServiceName = nameof(AuthService);
            options.AcceptEvent = e => e.IsEvent<UserInfoCreatedProfileIntegrationEvent, UserInfoUpdatedProfileIntegrationEvent>();
        }, kafkaSettings.UserConnectionName);
    }
    
    private static void AddOutboxService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<OutboxProcessor>();
        builder.AddOutboxBackgroundService();
        builder.AddOutboxRetryBackgroundService(option => option.RetryCount = 1);
    }
}
