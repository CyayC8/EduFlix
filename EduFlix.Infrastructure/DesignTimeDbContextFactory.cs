using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EduFlix.Infrastructure;

// Enkel voor 'dotnet ef' (design-time). Geeft een dummy connectie zodat we migraties
// kunnen aanmaken zonder draaiende database. Op runtime levert Aspire de echte connectie.
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=eduflixdb;Username=postgres;Password=postgres")
            .Options;

        return new ApplicationDbContext(options);
    }
}
