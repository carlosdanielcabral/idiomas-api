using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using IdiomasAPI.Source.Interface.Storage;

namespace IdiomasAPI.Source.Infrastructure.Storage.Adapter;

public class AzureBlobStorage(BlobServiceClient client, IConfiguration configuration, IWebHostEnvironment environment) : IFileStorageAdapter
{
    private readonly BlobServiceClient _client = client;
    private readonly IConfiguration _configuration = configuration;
    private readonly IWebHostEnvironment _environment = environment;

    public async Task<string> GenerateUrlToUpload(string filename)
    {
        BlobSasBuilder sasBuilder = this.GetSasBuilder(filename);

        sasBuilder.SetPermissions(BlobSasPermissions.Create | BlobSasPermissions.Write);

        return await this.GetBlobContainerUri(filename, sasBuilder);
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

    private async Task<string> GetBlobContainerUri(string filename, BlobSasBuilder sasBuilder)
    {
        BlobClient blobClient = this.GetBlobContainerClient().GetBlobClient(filename);

        if (this._environment.IsDevelopment())
        {
            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }

        UserDelegationKey userDelegationKey = await this._client.GetUserDelegationKeyAsync(
            startsOn: DateTimeOffset.UtcNow,
            expiresOn: DateTimeOffset.UtcNow.AddMinutes(5)
        );

        BlobSasQueryParameters sasQueryParameters = sasBuilder.ToSasQueryParameters(userDelegationKey, _client.AccountName);

        UriBuilder uriBuilder = new(blobClient.Uri)
        {
            Query = sasQueryParameters.ToString()
        };

        return uriBuilder.ToString();
    }
    
    private string GetContainerName()
    {
        return this._configuration["Azure:Storage:ContainerName"] ?? throw new InvalidOperationException("Container name is not configured.");
    }
}