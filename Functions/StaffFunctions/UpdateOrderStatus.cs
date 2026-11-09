using System.Text.Json;
using CoffeeAndChill.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeAndChill.Functions
{
    public class UpdateOrderStatusRequest
    {
        public string OrderDate { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class UpdateOrderStatus
    {
        private static readonly string[] AllowedStatuses =
            { "Received", "Preparing", "Ready", "Collected" };

        private readonly OrderTableService _orderTableService;

        public UpdateOrderStatus(OrderTableService orderTableService)
        {
            _orderTableService = orderTableService;
        }

        [Function("UpdateOrderStatus")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "orders/status")]
            HttpRequest req)
        {
            var body = await JsonSerializer.DeserializeAsync<UpdateOrderStatusRequest>(
                req.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (body == null ||
                string.IsNullOrWhiteSpace(body.OrderDate) ||
                string.IsNullOrWhiteSpace(body.OrderId) ||
                string.IsNullOrWhiteSpace(body.Status))
            {
                return new BadRequestObjectResult(new
                {
                    message = "orderDate, orderId and status are required."
                });
            }

            string? status = AllowedStatuses.FirstOrDefault(s =>
                s.Equals(body.Status.Trim(), StringComparison.OrdinalIgnoreCase));

            if (status == null)
            {
                return new BadRequestObjectResult(new
                {
                    message = "Status must be Received, Preparing, Ready or Collected."
                });
            }

            var existing = await _orderTableService.GetOrderAsync(body.OrderDate, body.OrderId);
            if (existing == null)
            {
                return new NotFoundObjectResult(new
                {
                    message = "Order not found.",
                    orderId = body.OrderId
                });
            }

            await _orderTableService.UpdateOrderStatusAsync(body.OrderDate, body.OrderId, status);

            return new OkObjectResult(new
            {
                message = "Order status updated.",
                orderId = body.OrderId,
                status
            });
        }
    }
}