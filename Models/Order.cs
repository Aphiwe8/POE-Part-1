using Azure;
using Azure.Data.Tables;
using System.Text.Json.Serialization;
namespace CoffeeAndChill.Models
{
    public class Order : ITableEntity
    {
        public string OrderId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public List<string> SelectedItemSKUs { get; set; } = new();
        public double TotalPrice { get; set; }
        public DateTimeOffset OrderTimestamp { get; set; }
        public string Status { get; set; } = "Received";

        [JsonIgnore] public string PartitionKey { get; set; } = string.Empty;
        [JsonIgnore] public string RowKey { get; set; } = string.Empty;
        [JsonIgnore] public DateTimeOffset? Timestamp { get; set; }
        [JsonIgnore] public ETag ETag { get; set; }
    }
}