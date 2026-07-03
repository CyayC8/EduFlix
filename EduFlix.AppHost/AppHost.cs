var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL draait als container. WithDataVolume houdt de data bij tussen restarts,
// en pgAdmin geeft een web-ui om de database te bekijken tijdens het ontwikkelen.
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var database = postgres.AddDatabase("eduflixdb");

builder.AddProject<Projects.EduFlix_Web>("web")
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();
