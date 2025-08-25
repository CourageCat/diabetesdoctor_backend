var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddConnectionString("kafka");

builder.AddProject<Projects.NotificationService_Web>("notificationservice-web").WithReference(kafka);

builder.Build().Run();
