using EduFlix.Domain;

namespace EduFlix.Application;

// Use-cases rond video's. Implementatie in Infrastructure (EF + blob).
public interface IVideoService
{
    Task<IReadOnlyList<Video>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Video>> GetByUploaderAsync(string uploaderId, CancellationToken ct = default);
    Task<Video?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Video> UploadAsync(VideoUploadRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, string title, string? description, int? categoryId, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken ct = default);
    Task IncrementDownloadCountAsync(Guid id, CancellationToken ct = default);
}

public record VideoUploadRequest(
    string Title,
    string? Description,
    int? CategoryId,
    string FileName,
    string ContentType,
    long SizeBytes,
    Stream Content,
    string UploadedById,
    int? DurationSeconds = null,
    byte[]? ThumbnailBytes = null);

// Regels waar elke upload aan moet voldoen, ook als iemand ooit rechtstreeks
// IVideoService.UploadAsync aanroept zonder via de Beheer-pagina te gaan.
public static class VideoUploadLimits
{
    public const long MaxFileSizeBytes = 500L * 1024 * 1024;
    public const string AllowedExtension = ".mp4";
}
