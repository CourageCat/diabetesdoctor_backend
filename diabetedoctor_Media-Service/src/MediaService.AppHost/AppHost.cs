var builder = DistributedApplication.CreateBuilder(args);
var kafka = builder.AddConnectionString("kafka");
builder.AddProject<Projects.MediaService_Web>("mediaservice-web").WithReference(kafka);

builder.Build().Run();
