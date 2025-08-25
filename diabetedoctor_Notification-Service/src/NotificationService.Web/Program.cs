using NotificationService.Application.DependencyInjection.Extensions;
using NotificationService.Infrastructure.DependencyInjection.Extensions;
using NotificationService.Persistence.DependencyInjection.Extensions;
using NotificationService.ServiceDefaults;
using NotificationService.Web.DependencyInjections.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddConfigurationService();
builder.AddWebService();
builder.AddPersistenceServices();
builder.AddApplicationService();
builder.AddInfrastructureService();
//builder.AddKafkaConfiguration();

var app = builder.Build();

app.MapDefaultEndpoints();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

//app.MapGet("/", () => "Hello World!");

app.ConfigureMiddleware();

app.Run();
