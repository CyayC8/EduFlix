using EduFlix.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduFlix.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Video> Videos => Set<Video>();
    public DbSet<VideoView> VideoViews => Set<VideoView>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Video>(e =>
        {
            e.Property(v => v.Title).HasMaxLength(200).IsRequired();
            e.HasIndex(v => v.UploadedById);
            e.HasOne(v => v.Category)
                .WithMany(c => c.Videos)
                .HasForeignKey(v => v.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<VideoView>(e =>
        {
            e.HasIndex(v => v.VideoId);
            e.HasIndex(v => v.ViewedAt);
            e.HasOne(v => v.Video)
                .WithMany(v => v.Views)
                .HasForeignKey(v => v.VideoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Category>(e =>
        {
            e.Property(c => c.Name).HasMaxLength(100).IsRequired();
            e.HasIndex(c => c.Name).IsUnique();
        });
    }
}
