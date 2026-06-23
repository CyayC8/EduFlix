var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.EduFlix_Web>("web");

builder.Build().Run();
