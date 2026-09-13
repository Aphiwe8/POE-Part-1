using Azure.Storage.Blobs;

namespace CoffeeAndChill.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService()
        {
            string connectionString =
                "DefaultEndpointsProtocol=http;" +
                "AccountName=devstoreaccount1;" +
                "AccountKey=Eby8vdM02xNOcqFlqUwJv8h6w5h1L5e6b5w3Z2Q1V9X0Y8W7U6T5S4R3Q2P1O0N;" +
                "BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;";

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