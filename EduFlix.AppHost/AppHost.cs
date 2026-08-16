var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL draait als container. WithDataVolume houdt de data bij tussen restarts,
// en pgAdmin geeft een web-ui om de database te bekijken tijdens het ontwikkelen.

// Aspire genereert en beheert zelf een veilig wachtwoord (zichtbaar in de dashboard),
// geen handmatige user-secrets nodig.
var postgres = builder.AddPostgres("postgres2")
    .WithDataVolume()
    .WithPgAdmin();

var database = postgres.AddDatabase("eduflixdb");

// Azure Blob Storage voor de videobestanden. In dev lokaal geemuleerd met Azurite (container).
// WithDataVolume zorgt dat geuploade bestanden een herstart overleven (zelfde patroon als bij postgres).
var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator(azurite => azurite.WithDataVolume());

var blobs = storage.AddBlobs("blobs");

// Migreert de database en seedt rollen/lecturer-account. Draait 1x en stopt dan zichzelf;
// 'web' wacht tot dit volledig klaar is voor die zelf opstart.
var migrations = builder.AddProject<Projects.EduFlix_MigrationService>("migrations")
    .WithReference(database)
    .WaitFor(database);

builder.AddProject<Projects.EduFlix_Web>("web")
    .WithReference(database)
    .WithReference(blobs)
    .WaitFor(database)
    .WaitFor(blobs)
    .WaitForCompletion(migrations);

builder.Build().Run();
