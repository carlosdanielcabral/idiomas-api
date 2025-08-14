using Azure.Identity;
using Azure.Storage.Blobs;
using Idiomas.Source.Infrastructure.Storage.Adapter;
using Idiomas.Source.Interface.Storage;

namespace Idiomas.Source.Infrastructure.Storage;

public static class DependencyInjection
{
    public static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            string connectionString = configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException("Connection string for AzureStorage is not configured");

            services.AddSingleton(x => new BlobServiceClient(connectionString));
        }
        else
        {
            string blobServiceUri = configuration["Azure:Storage:BlobServiceUri"]
                ?? throw new InvalidOperationException("Blob Service Uri is not configured.");

            services.AddSingleton(x =>
                new BlobServiceClient(new Uri(blobServiceUri), new DefaultAzureCredential())
            );
        }

        services.AddScoped<IFileStorageAdapter, AzureBlobStorage>();
        services.AddScoped<IFileStorage, FileStorage>();

        return services;
    }
}