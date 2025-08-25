using Microsoft.AspNetCore.Builder;

namespace MediaService.Persistence.DependencyInjection.Extensions;

public static class DatabaseExtention
{
    //public static async Task InitialiseDatabaseAsync(this WebApplication app, IConfiguration configuration, IServiceProvider serviceProvider)
    //{
    //    using var scope = app.Services.CreateScope();

    //    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //    var initialData = scope.ServiceProvider.GetRequiredService<InitialData>();
    //    context.Database.MigrateAsync().GetAwaiter().GetResult();

    //    await SeedAsync(context, initialData, configuration, serviceProvider);
    //}

    //private static async Task SeedAsync(ApplicationDbContext context, InitialData initialData, IConfiguration configuration, IServiceProvider serviceProvider)
    //{ }
}
