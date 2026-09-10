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
        public async Task<List<MenuItem>> GetAllMenuItemsAsync()
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            await foreach (TableEntity entity in _tableClient.QueryAsync<TableEntity>())
            {
                MenuItem menuItem = new MenuItem
                {
                    Category = entity.PartitionKey,
                    Id = entity.RowKey,
                    Name = entity.GetString("Name") ?? string.Empty,
                    Description = entity.GetString("Description") ?? string.Empty,
                    Price = entity.GetDouble("Price") ?? 0,
                    IsAvailable = entity.GetBoolean("IsAvailable") ?? false
                };

                menuItems.Add(menuItem);
            }

            return menuItems;
        }
        public async Task<List<MenuItem>> GetMenuItemsByCategoryAsync(string category)
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            await foreach (TableEntity entity in _tableClient.QueryAsync<TableEntity>(
                filter: $"PartitionKey eq '{category}'"))
            {
                MenuItem menuItem = new MenuItem
                {
                    Category = entity.PartitionKey,
                    Id = entity.RowKey,
                    Name = entity.GetString("Name") ?? string.Empty,
                    Description = entity.GetString("Description") ?? string.Empty,
                    Price = entity.GetDouble("Price") ?? 0,
                    IsAvailable = entity.GetBoolean("IsAvailable") ?? false
                };

                menuItems.Add(menuItem);
            }

            return menuItems;
        }
        public async Task UpdateMenuItemAsync(MenuItem menuItem)
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

            await _tableClient.UpdateEntityAsync(
                entity,
                ETag.All,
                TableUpdateMode.Replace);
        }
        public async Task DeleteMenuItemAsync(string category, string id)
        {
            await _tableClient.DeleteEntityAsync(
                category,
                id);
        }

    }
}

//Azure.Storage.Blobs
//Azure.Storage.Common