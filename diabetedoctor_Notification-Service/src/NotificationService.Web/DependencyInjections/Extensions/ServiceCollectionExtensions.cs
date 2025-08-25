using NotificationService.Contract.EventBus.Abstractions;
using NotificationService.Contract.EventBus.Abstractions.Message;
using NotificationService.Domain;
using NotificationService.Infrastructure.Idempotence;
using NotificationService.Infrastructure.Outbox;

namespace NotificationService.Web.DependencyInjections.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddConfigurationService(this IHostApplicationBuilder builder)
    {
        builder.Services
            .Configure<MongoDbSettings>(builder.Configuration.GetSection(MongoDbSettings.SectionName))
            .Configure<AuthSettings>(builder.Configuration.GetSection(AuthSettings.SectionName))
            .Configure<AblySettings>(builder.Configuration.GetSection(AblySettings.SectionName))
            .Configure<KafkaSettings>(builder.Configuration.GetSection(KafkaSettings.SectionName));
    }
    public static void AddConfigureMediatR(this IServiceCollection services)
        => services.AddMediatR(config => config.RegisterServicesFromAssembly(AssemblyReference.Assembly))
                   .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
                   .AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);
    
    private static void AddSwaggerServices(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddSwaggerGenNewtonsoftSupport()
            .AddFluentValidationRulesToSwagger()
            .AddEndpointsApiExplorer()
            .AddSwagger();
    }
    
    private static void AddAuthenticationAndAuthorization(this IHostApplicationBuilder builder)
    {
        var authSettings = builder.Configuration.GetSection(AuthSettings.SectionName).Get<AuthSettings>() ?? new AuthSettings();


        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authSettings.Issuer,
                    ValidAudience = authSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.AccessSecretToken)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Append("IS-TOKEN-EXPIRED", "true");
                        }
                        return Task.CompletedTask;
                    },
                };
            });

        builder.Services.AddAuthorizationBuilder();
    }
    
    //public static void AddKafkaConfiguration(this IHostApplicationBuilder builder)
    //{
    //    builder.AddKafkaSettingEventBus();
    //    builder.AddKafkaProducer();
    //    builder.AddKafkaEventPublisher();
    //    builder.AddKafkaConsumer();

    //    builder.Services.AddSingleton(typeof(IntegrationEventFactory));
    //}

    public static void AddWebService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddCarter();

        builder.Services.AddHttpContextAccessor();

        builder.AddAuthenticationAndAuthorization();

        builder.Services.AddScoped<ExceptionHandlingMiddleware>();

        builder.AddSwaggerServices();

        builder.Services
            .AddApiVersioning(options => options.ReportApiVersions = true)
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
    }
}
