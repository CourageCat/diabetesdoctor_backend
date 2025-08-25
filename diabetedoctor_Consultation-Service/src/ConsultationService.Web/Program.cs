using ConsultationService.Application.DependencyInjection.Extensions;
using ConsultationService.Infrastructure.DependencyInjection.Extensions;
using ConsultationService.Persistence.DependencyInjection.Extensions;
using ConsultationService.ServiceDefaults;
using ConsultationService.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddWebService();
builder.AddApplicationService();
builder.AddInfrastructureService();
builder.AddPersistenceServices();

var app = builder.Build();

app.MapDefaultEndpoints();

app.ConfigureMiddleware();

app.Run();
