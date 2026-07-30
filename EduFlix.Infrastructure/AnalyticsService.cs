using EduFlix.Application;
using EduFlix.Domain;
using Microsoft.EntityFrameworkCore;

namespace EduFlix.Infrastructure;

public class AnalyticsService(ApplicationDbContext db) : IAnalyticsService
{
    public async Task LogViewAsync(Guid videoId, string viewerId, CancellationToken ct = default)
    {
        db.VideoViews.Add(new VideoView { VideoId = videoId, ViewerId = viewerId });
        await db.SaveChangesAsync(ct);
    }

    public async Task<int> GetViewCountAsync(Guid videoId, CancellationToken ct = default)
        => await db.VideoViews.CountAsync(v => v.VideoId == videoId, ct);
}
