using Azure.Storage.Blobs;

namespace CoffeeAndChill.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService()
        {
            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? throw new InvalidOperationException(
                    "AzureWebJobsStorage connection string is missing.");

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