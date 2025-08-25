using ConsultationService.Contract.Settings;
using ConsultationService.Domain.Abstractions;
using ConsultationService.Domain.Abstractions.Repositories;
using ConsultationService.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsultationService.Persistence.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        
        builder.AddDatabaseConfiguration();
        builder.AddConfigurationService(builder.Configuration);
        
        builder.Services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IMediaRepository, MediaRepository>()
            .AddScoped<IHospitalRepository, HospitalRepository>()
            .AddScoped<IConsultationTemplateRepository, ConsultationTemplateRepository>()
            .AddScoped<IConsultationRepository, ConsultationRepository>()
            .AddScoped<IOutboxEventRepository, OutboxEventRepository>()
            .AddScoped<IOutboxScheduleEventRepository, OutboxScheduleEventRepository>()
            .AddScoped<IOutBoxEventConsumerRepository, OutBoxEventConsumerRepository>();
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
