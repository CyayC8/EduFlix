using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduFlix.Web.Data;

// Migreert de database en zorgt dat de rollen + een lecturer-account bestaan.
public static class IdentitySeed
{
    public const string StudentRole = "Student";
    public const string LecturerRole = "Lecturer";

    public static async Task RunAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        var db = sp.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { StudentRole, LecturerRole })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        const string email = "lecturer@eduflix.be";
        if (await userManager.FindByEmailAsync(email) is null)
        {
            var lecturer = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(lecturer, "Lecturer123!");
            await userManager.AddToRoleAsync(lecturer, LecturerRole);
        }
    }
}
