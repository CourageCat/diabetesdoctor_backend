using System.Reflection;
using ConsultationService.Application.Behaviors;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsultationService.Application.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    private static IServiceCollection AddConfigureMediatR(this IServiceCollection services)
    {
        return services.AddMediatR(config => config.RegisterServicesFromAssemblies(
                AssemblyReference.Assembly,
                Assembly.Load("ConsultationService.Infrastructure")
            ))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
            //.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            //.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>))
            .AddValidatorsFromAssembly(Contract.AssemblyReference.Assembly, includeInternalTypes: true);
    }

    private static IServiceCollection AddMappingConfig(this IServiceCollection services)
    {
        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        return services;
    }

    public static void AddApplicationService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMappingConfig();
        builder.Services.AddConfigureMediatR();
    }


}