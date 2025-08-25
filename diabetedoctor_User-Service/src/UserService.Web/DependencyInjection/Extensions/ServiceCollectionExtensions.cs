using Microsoft.AspNetCore.Http.Json;
using UserService.Contract.Settings;

namespace UserService.Api.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions 
{
    public static IServiceCollection AddConfigurationAppSetting
     (this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<AppDefaultSettings>(configuration.GetSection(AppDefaultSettings.SectionName))
            .Configure<KafkaSettings>(configuration.GetSection(KafkaSettings.SectionName))
            .Configure<RedisSettings>(configuration.GetSection(RedisSettings.SectionName))
            .Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName))
            .Configure<GeminiSettings>(configuration.GetSection(GeminiSettings.SectionName))
            .Configure<PayOSSettings>(configuration.GetSection(PayOSSettings.SectionName));
        return services;
    }
    
    private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services
            .AddSwaggerGenNewtonsoftSupport()
            .AddEndpointsApiExplorer()
            .AddSwagger();

        return services;
    }

    public static void ConfigureApiService(this IHostApplicationBuilder builder)
    {
        builder.AddConfigureGrpcServices();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddConfigurationAppSetting(builder.Configuration);

        builder.Services.AddCarter();

        builder.Services
             .AddTransient<ExceptionHandlingMiddleware>()
             .AddTransient<RequireRoleMiddleware>();

        builder.Services.AddSwaggerServices();

        builder.Services.AddHttpClient("AiService", client =>
        {
            client.BaseAddress = new Uri("http://127.0.0.1:8000");
        });

        builder.Services
            .AddApiVersioning(options => options.ReportApiVersions = true)
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });
    }
    
    private static void AddConfigureGrpcServices(this IHostApplicationBuilder builder)
    {
        // Client
        
        // Server
        builder.Services.AddGrpc();
    }
}