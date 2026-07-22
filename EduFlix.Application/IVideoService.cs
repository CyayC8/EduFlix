using EduFlix.Domain;

namespace EduFlix.Application;

// Use-cases rond video's. Implementatie in Infrastructure (EF + blob).
public interface IVideoService
{
    Task<IReadOnlyList<Video>> GetAllAsync(CancellationToken ct = default);
    Task<Video?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Video> UploadAsync(VideoUploadRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, string title, string? description, int? categoryId, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken ct = default);
}

public record VideoUploadRequest(
    string Title,
    string? Description,
    int? CategoryId,
    string FileName,
    string ContentType,
    long SizeBytes,
    Stream Content,
    string UploadedById);
