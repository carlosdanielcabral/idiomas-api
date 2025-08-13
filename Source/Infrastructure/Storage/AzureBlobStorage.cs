using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using IdiomasAPI.Source.Interface.Storage;

namespace IdiomasAPI.Source.Infrastructure.Storage.Adapter;

public class AzureBlobStorage(BlobServiceClient client, IConfiguration configuration) : IFileStorage
{
    private readonly BlobServiceClient _client = client;
    private readonly IConfiguration _configuration = configuration;

    public async Task<string> GenerateUrlToUpload(string filename)
    {
        BlobSasBuilder sasBuilder = this.GetSasBuilder(filename);

        sasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write);

        BlobSasQueryParameters blobSasQueryParameters = sasBuilder.ToSasQueryParameters(
            await this.GetDelegationKey(),
            this._client.AccountName
        );

        return this.GetBlobContainerUriBuilder(filename, blobSasQueryParameters).Uri.ToString();
    }

    private async Task<Azure.Response<UserDelegationKey>> GetDelegationKey()
    {
        return await this._client.GetUserDelegationKeyAsync(
            startsOn: DateTimeOffset.UtcNow,
            expiresOn: DateTimeOffset.UtcNow.AddMinutes(1)
        );
    }

    private BlobSasBuilder GetSasBuilder(string filename)
    {
        return new BlobSasBuilder()
        {
            BlobContainerName = this.GetContainerName(),
            BlobName = filename,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(1)
        };
    }

    private BlobContainerClient GetBlobContainerClient()
    {
        return this._client.GetBlobContainerClient(this.GetContainerName());
    }

    private UriBuilder GetBlobContainerUriBuilder(string filename, BlobSasQueryParameters blobSasQueryParameters)
    {
        BlobClient blobClient = this.GetBlobContainerClient().GetBlobClient(filename);

        return new UriBuilder(blobClient.Uri) { Query = blobSasQueryParameters.ToString() };
    }
    
    private string GetContainerName()
    {
        return this._configuration["Azure:Storage:ContainerName"] ?? throw new InvalidOperationException("Container name is not configured.");
    }
}