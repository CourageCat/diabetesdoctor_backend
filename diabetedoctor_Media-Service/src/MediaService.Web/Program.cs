using MediaService.Application.DependencyInjection.Extensions;
using MediaService.Domain.Abstractions;
using MediaService.Infrastructure.Services;
using MediaService.Persistence.SeedData;
using MediaService.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddWebService();
builder.AddInfrastructureService();
builder.AddApplicationService();
builder.AddPersistenceServices();

var app = builder.Build();

app.MapDefaultEndpoints();
// Seed Data
// using (var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<IMongoDbContext>();
//     var seeder = new SeedData(context);
//     await seeder.SeedAsync(); // Use instance method
// }
app.ConfigureMiddleware();
app.Run();