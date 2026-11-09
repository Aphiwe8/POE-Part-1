using Azure;
using Azure.Data.Tables;
using CoffeeAndChill.ApiGateway.Models;

namespace CoffeeAndChill.ApiGateway.Services
{
    public class UserTableService
    {
        private readonly TableClient _tableClient;

        public UserTableService(IConfiguration configuration)
        {
            string connectionString =
                configuration["AzureStorage:ConnectionString"]
                ?? "UseDevelopmentStorage=true";

            var serviceClient = new TableServiceClient(connectionString);

            _tableClient = serviceClient.GetTableClient("Users");

            _tableClient.CreateIfNotExists();
        }

        public async Task<UserEntity?> GetUserByEmailAsync(string email)
        {
            string normalizedEmail = email.Trim().ToLowerInvariant();

            try
            {
                return await _tableClient.GetEntityAsync<UserEntity>(
                    "USERS",
                    normalizedEmail
                );
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task CreateUserAsync(UserEntity user)
        {
            user.PartitionKey = "USERS";
            user.RowKey = user.Email.Trim().ToLowerInvariant();

            await _tableClient.AddEntityAsync(user);
        }
    }
}