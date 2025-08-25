using AuthService.Api.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();

builder.ConfigureApiService();
builder.ConfigureApplicationServices();
builder.ConfigurePersistenceServices();
builder.ConfigureInfrastructureService();

var app = builder.Build();

app.ConfigureMiddleware();

app.UseHttpsRedirection();

app.Run();