namespace EduFlix.Application;

// Bijhouden en opvragen van kijkcijfers. Uitgebreider dashboard komt in een latere fase.
public interface IAnalyticsService
{
    Task LogViewAsync(Guid videoId, string viewerId, CancellationToken ct = default);
    Task<int> GetViewCountAsync(Guid videoId, CancellationToken ct = default);
}
