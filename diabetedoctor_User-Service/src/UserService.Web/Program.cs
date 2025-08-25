using UserService.Api.DependencyInjection.Extensions;
using UserService.Application.DependencyInjection.Extensions;
using UserService.Infrastructure.DependencyInjection.Extensions;
using UserService.Persistence.DependencyInjection.Extensions;
using UserService.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.ConfigureApiService();
builder.ConfigureApplicationService();
builder.ConfigurePersistenceServices();
builder.ConfigureInfrastructureService();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
}

// // Seed data
// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     var context = services.GetRequiredService<ApplicationDbContext>();
//
//     SeedData.Seed(context, builder.Configuration);
// }

app.ConfigureMiddleware();

app.UseHttpsRedirection();

app.Run();