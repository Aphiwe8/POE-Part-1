using Azure.Data.Tables;
using CoffeeAndChill.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeAndChill.Functions
{
    public class GetOrderStatus
    {
        private readonly OrderTableService _orderTableService;

        public GetOrderStatus(OrderTableService orderTableService)
        {
            _orderTableService = orderTableService;
        }

        [Function("GetOrderStatus")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "orders/{orderDate}/{orderId}")]
            HttpRequest req,
            string orderDate,
            string orderId)
        {
            TableEntity? order =
                await _orderTableService.GetOrderAsync(
                    orderDate,
                    orderId);

            if (order == null)
            {
                return new NotFoundObjectResult(new
                {
                    message = "Order not found.",
                    orderId = orderId
                });
            }

            return new OkObjectResult(new
            {
                orderId = order.GetString("OrderId"),
                customerName = order.GetString("CustomerName"),
                selectedItemSKUs =
                    order.GetString("SelectedItemSKUs"),
                totalPrice =
                    order.GetDouble("TotalPrice"),
                orderTimestamp =
                    order.GetDateTimeOffset("OrderTimestamp"),
                status =
                    order.GetString("Status"),
                partitionKey = order.PartitionKey,
                rowKey = order.RowKey
            });
        }
    }
}