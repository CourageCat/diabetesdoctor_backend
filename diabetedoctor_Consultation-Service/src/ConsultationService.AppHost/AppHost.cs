var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.ConsultationService_Web>("apiservice")
    .WithHttpHealthCheck("/health");
//
builder.AddProject<Projects.ConsultationService_Web>("consultationserivce-web")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();