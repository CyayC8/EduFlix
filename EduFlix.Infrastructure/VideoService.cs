using EduFlix.Application;
using EduFlix.Domain;
using Microsoft.EntityFrameworkCore;

namespace EduFlix.Infrastructure;

public class VideoService(ApplicationDbContext db, IBlobStorage blobs) : IVideoService
{
    public async Task<IReadOnlyList<Video>> GetAllAsync(CancellationToken ct = default)
        => await db.Videos.Include(v => v.Category).OrderByDescending(v => v.CreatedAt).ToListAsync(ct);

    public async Task<IReadOnlyList<Video>> GetByUploaderAsync(string uploaderId, CancellationToken ct = default)
        => await db.Videos.Include(v => v.Category)
            .Where(v => v.UploadedById == uploaderId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync(ct);

    public async Task<Video?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Videos.Include(v => v.Category).FirstOrDefaultAsync(v => v.Id == id, ct);

    public async Task<Video> UploadAsync(VideoUploadRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new InvalidOperationException("Titel is verplicht.");

        var extension = Path.GetExtension(request.FileName);
        if (!extension.Equals(VideoUploadLimits.AllowedExtension, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Enkel MP4-bestanden zijn toegelaten.");

        if (request.SizeBytes > VideoUploadLimits.MaxFileSizeBytes)
            throw new InvalidOperationException("Bestand is te groot (max 500 MB).");

        var id = Guid.NewGuid();
        var blobName = $"{id}{extension}";

        await blobs.UploadAsync(blobName, request.Content, request.ContentType, ct);

        string? thumbnailBlobName = null;
        if (request.ThumbnailBytes is { Length: > 0 })
        {
            thumbnailBlobName = $"{id}-thumb.jpg";
            using var thumbnailStream = new MemoryStream(request.ThumbnailBytes);
            await blobs.UploadAsync(thumbnailBlobName, thumbnailStream, "image/jpeg", ct);
        }

        var video = new Video
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            BlobName = blobName,
            ContentType = request.ContentType,
            SizeBytes = request.SizeBytes,
            UploadedById = request.UploadedById,
            DurationSeconds = request.DurationSeconds,
            ThumbnailBlobName = thumbnailBlobName,
        };

        db.Videos.Add(video);
        await db.SaveChangesAsync(ct);
        return video;
    }

    public async Task UpdateAsync(Guid id, string title, string? description, int? categoryId, CancellationToken ct = default)
    {
        var video = await db.Videos.FirstOrDefaultAsync(v => v.Id == id, ct)
            ?? throw new InvalidOperationException($"Video {id} niet gevonden.");

        video.Title = title;
        video.Description = description;
        video.CategoryId = categoryId;
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var video = await db.Videos.FirstOrDefaultAsync(v => v.Id == id, ct);
        if (video is null) return;

        await blobs.DeleteAsync(video.BlobName, ct);
        if (video.ThumbnailBlobName is not null)
        {
            await blobs.DeleteAsync(video.ThumbnailBlobName, ct);
        }
        db.Videos.Remove(video);
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken ct = default)
        => await db.Categories.OrderBy(c => c.Name).ToListAsync(ct);

    public async Task IncrementDownloadCountAsync(Guid id, CancellationToken ct = default)
    {
        var video = await db.Videos.FirstOrDefaultAsync(v => v.Id == id, ct);
        if (video is null) return;

        video.DownloadCount++;
        await db.SaveChangesAsync(ct);
    }
}
