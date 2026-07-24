using EduFlix.Domain;
using Microsoft.EntityFrameworkCore;

namespace EduFlix.Infrastructure;

public static class CategorySeed
{
    private static readonly string[] Defaults = ["Algemeen", "Programmeren", "Wiskunde", "Talen"];

    public static async Task RunAsync(ApplicationDbContext db)
    {
        if (await db.Categories.AnyAsync()) return;

        db.Categories.AddRange(Defaults.Select(name => new Category { Name = name }));
        await db.SaveChangesAsync();
    }
}
