namespace EduFlix.Domain;

public class Video
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Opslag in blob storage
    public string BlobName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public int? DurationSeconds { get; set; }
    public string? ThumbnailBlobName { get; set; }

    // Wie uploadde: het Identity user-id (geen navigatie naar Identity, zo blijft Domain zuiver)
    public string UploadedById { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public int DownloadCount { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    // Analytics: elke kijkbeurt is een aparte rij (event-log)
    public ICollection<VideoView> Views { get; set; } = new List<VideoView>();
}
