using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using EduFlix.Application;

namespace EduFlix.Infrastructure;

// Azure Blob Storage (lokaal geëmuleerd via Azurite). Alle video-bestanden staan
// in de container "videos"; de BlobServiceClient komt van Aspire (AddAzureBlobClient in Web).
// SAS-urls worden ondertekend met een shared key (lokaal) of een user delegation key via
// Azure AD (cloud, waar de client via Managed Identity verbindt, zonder shared key).
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

    public async Task<Uri> GetReadSasUriAsync(string blobName, TimeSpan validFor, CancellationToken ct = default)
    {
        var blob = GetContainer().GetBlobClient(blobName);
        var startsOn = DateTimeOffset.UtcNow.AddMinutes(-5); // marge voor kloktijd-verschillen
        var expiresOn = DateTimeOffset.UtcNow.Add(validFor);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = ContainerName,
            BlobName = blobName,
            Resource = "b",
            StartsOn = startsOn,
            ExpiresOn = expiresOn
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        if (blob.CanGenerateSasUri)
        {
            // lokaal (Azurite): de client heeft een shared key, dus kan zelf rechtstreeks ondertekenen
            return blob.GenerateSasUri(sasBuilder);
        }

        // cloud: de client authenticeert via Managed Identity (geen shared key), dus
        // vragen we een tijdelijke "user delegation key" op bij Azure AD om de SAS mee te ondertekenen
        var userDelegationKey = await client.GetUserDelegationKeyAsync(startsOn, expiresOn, ct);
        var sasQueryParams = sasBuilder.ToSasQueryParameters(userDelegationKey.Value, client.AccountName);

        var uriBuilder = new BlobUriBuilder(blob.Uri) { Sas = sasQueryParams };
        return uriBuilder.ToUri();
    }
}
