namespace UserService.Persistence.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        // services.AddSingleton<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        var connectionString = configuration["ConnectionStrings:DatabaseConnectionString"];
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetRequiredService<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });
    }

    public static void ConfigurePersistenceServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseConfiguration(builder.Configuration);

        builder.Services
            .AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork))
            .AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>))
            .AddScoped(typeof(IOutboxEventConsumerRepository), typeof(OutboxEventConsumerRepository))
            .AddScoped(typeof(IServicePackageRepository), typeof(ServicePackageRepository))
            .AddScoped(typeof(IUserPackageRepository), typeof(UserPackageRepository));
    }
}
