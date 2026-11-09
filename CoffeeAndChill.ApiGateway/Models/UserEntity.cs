using Azure;
using Azure.Data.Tables;

namespace CoffeeAndChill.ApiGateway.Models
{
    public class UserEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = "USERS";

        public string RowKey { get; set; } = string.Empty;

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public string UserId { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Staff";

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}