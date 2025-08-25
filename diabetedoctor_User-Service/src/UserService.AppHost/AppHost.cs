using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var kafka = builder.AddConnectionString("kafka");
builder.AddProject<UserService_Web>("userservice-web").WithReference(kafka);

builder.Build().Run();
