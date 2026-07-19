namespace EduFlix.Domain;

// Eén rij per kijkbeurt (event-log) — bron voor de analytics.
public class VideoView
{
    public long Id { get; set; }

    public Guid VideoId { get; set; }
    public Video Video { get; set; } = null!;

    // Welke gebruiker keek (Identity user-id)
    public string ViewerId { get; set; } = string.Empty;

    public DateTimeOffset ViewedAt { get; set; } = DateTimeOffset.UtcNow;
}
