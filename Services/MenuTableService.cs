using Azure;
using Azure.Data.Tables;
using CoffeeAndChill.Models;

namespace CoffeeAndChill.Services
{
    public class MenuTableService
    {
        private readonly TableClient _tableClient;

        public MenuTableService()
        {
            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? throw new InvalidOperationException(
                    "AzureWebJobsStorage connection string is missing.");

            _tableClient = new TableClient(
                connectionString,
                "MenuItems");
        }

        public async Task CreateTableAsync()
        {
            await _tableClient.CreateIfNotExistsAsync();
        }

        public async Task CreateMenuItemAsync(MenuItem menuItem)
        {
            TableEntity entity = new TableEntity
            {
                PartitionKey = menuItem.Category,
                RowKey = menuItem.Id,

                ["Name"] = menuItem.Name,
                ["Description"] = menuItem.Description,
                ["Price"] = menuItem.Price,
                ["IsAvailable"] = menuItem.IsAvailable
            };

            await _tableClient.AddEntityAsync(entity);
        }
    }
}

//Azure.Storage.Blobs
//Azure.Storage.Common