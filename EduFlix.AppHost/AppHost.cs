var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL draait als container. WithDataVolume houdt de data bij tussen restarts,
// en pgAdmin geeft een web-ui om de database te bekijken tijdens het ontwikkelen.
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var database = postgres.AddDatabase("eduflixdb");

// Azure Blob Storage voor de videobestanden. In dev lokaal geemuleerd met Azurite (container).
var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator();

var blobs = storage.AddBlobs("blobs");

builder.AddProject<Projects.EduFlix_Web>("web")
    .WithReference(database)
    .WithReference(blobs)
    .WaitFor(database)
    .WaitFor(blobs);

builder.Build().Run();
