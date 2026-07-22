using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using EduFlix.Application;

namespace EduFlix.Infrastructure;

// Azure Blob Storage (lokaal geëmuleerd via Azurite). Alle video-bestanden staan
// in de container "videos"; de BlobServiceClient komt van Aspire (AddAzureBlobClient in Web).
public class BlobStorage(BlobServiceClient client) : IBlobStorage
{
    private const string ContainerName = "videos";

    private BlobContainerClient GetContainer()
        => client.GetBlobContainerClient(ContainerName);

    public async Task<string> UploadAsync(string blobName, Stream content, string contentType, CancellationToken ct = default)
    {
        var container = GetContainer();
        await container.CreateIfNotExistsAsync(cancellationToken: ct);

        var blob = container.GetBlobClient(blobName);
        await blob.UploadAsync(content, new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);
        return blobName;
    }

    public async Task<Stream> OpenReadAsync(string blobName, CancellationToken ct = default)
    {
        var blob = GetContainer().GetBlobClient(blobName);
        return await blob.OpenReadAsync(cancellationToken: ct);
    }

    public async Task DeleteAsync(string blobName, CancellationToken ct = default)
    {
        var blob = GetContainer().GetBlobClient(blobName);
        await blob.DeleteIfExistsAsync(cancellationToken: ct);
    }

    public Uri GetReadSasUri(string blobName, TimeSpan validFor)
    {
        var blob = GetContainer().GetBlobClient(blobName);

        if (!blob.CanGenerateSasUri)
        {
            // val terug op de kale url (kan gebeuren als de client geen shared key heeft)
            return blob.Uri;
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = ContainerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(validFor)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return blob.GenerateSasUri(sasBuilder);
    }
}
