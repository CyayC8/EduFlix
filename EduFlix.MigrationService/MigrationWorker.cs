using EduFlix.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EduFlix.MigrationService;

// Draait de EF-migraties en de seeding (rollen + lecturer-account) EER de rest van de app
// opstart. De AppHost laat 'web' wachten tot dit proces klaar is (WaitForCompletion).
public class MigrationWorker(IServiceProvider services, IHostApplicationLifetime lifetime) : BackgroundService
{
    public const string ActivitySourceName = "EduFlix.MigrationService";
    private static readonly System.Diagnostics.ActivitySource ActivitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity("Migrating database", System.Diagnostics.ActivityKind.Client);

        try
        {
            await IdentitySeed.RunAsync(services);

            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await CategorySeed.RunAsync(db);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        // klaar: dit proces heeft geen langlevende taak, sluit netjes af
        lifetime.StopApplication();
    }
}
