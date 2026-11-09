using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace CoffeeAndChill.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(IConfiguration configuration)
        {
            string connectionString =
     configuration["AzureWebJobsStorage"]
     ?? throw new InvalidOperationException("AzureWebJobsStorage connection string is missing.");

            BlobServiceClient blobServiceClient =
                new BlobServiceClient(connectionString);

            _containerClient =
                blobServiceClient.GetBlobContainerClient("staff-docs");
        }

        public async Task CreateContainerAsync()
        {
            await _containerClient.CreateIfNotExistsAsync();
        }
    }
}