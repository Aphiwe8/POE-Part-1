using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace CoffeeAndChill.Services
{
    public class StaffDocumentService
    {
        private readonly ShareClient _shareClient;

        public StaffDocumentService()
        {
            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? throw new InvalidOperationException(
                    "AzureWebJobsStorage connection string is missing.");

            _shareClient = new ShareClient(
                connectionString,
                "staff-docs");
        }

        public async Task CreateShareAsync()
        {
            await _shareClient.CreateIfNotExistsAsync();
        }
    }
}