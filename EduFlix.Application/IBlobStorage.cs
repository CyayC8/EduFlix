namespace EduFlix.Application;

// Contract voor de opslag van videobestanden.
// Geïmplementeerd in Infrastructure met Azure Blob Storage (lokaal geëmuleerd via Azurite).
public interface IBlobStorage
{
    Task<string> UploadAsync(string blobName, Stream content, string contentType, CancellationToken ct = default);
    Task<Stream> OpenReadAsync(string blobName, CancellationToken ct = default);
    Task DeleteAsync(string blobName, CancellationToken ct = default);

    // Tijdelijke, ondertekende url zodat de browser rechtstreeks vanaf storage kan streamen.
    Uri GetReadSasUri(string blobName, TimeSpan validFor);
}
