namespace UserService.Application.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigureMediatR(this IServiceCollection services)
    => services.AddMediatR(config => config.RegisterServicesFromAssemblies(
                Assembly.GetExecutingAssembly(),
                Assembly.Load("UserService.Application"),
                Assembly.Load("UserService.Persistence"),
                Assembly.Load("UserService.Infrastructure")
            ))
          .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
          .AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>))
          .AddValidatorsFromAssembly(Contract.AssemblyReference.Assembly, includeInternalTypes: true);

    public static void ConfigureApplicationService(this IHostApplicationBuilder builder)
    {
        builder.Services.AddConfigureMediatR();
        builder.Services.AddQuartzJobs();
    }
}