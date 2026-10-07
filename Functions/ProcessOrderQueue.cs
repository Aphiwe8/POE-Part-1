using System.Text.Json;
using Azure.Storage.Queues.Models;
using CoffeeAndChill.Models;
using CoffeeAndChill.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeAndChill.Functions
{
    public class ProcessOrderQueue
    {
        private readonly OrderTableService _orderTableService;
        private readonly ILogger<ProcessOrderQueue> _logger;

        public ProcessOrderQueue(
            OrderTableService orderTableService,
            ILogger<ProcessOrderQueue> logger)
        {
            _orderTableService = orderTableService;
            _logger = logger;
        }

        [Function("ProcessOrderQueue")]
        public async Task Run(
            [QueueTrigger(
                "order-processing-queue",
                Connection = "AzureWebJobsStorage")]
            QueueMessage message)
        {
            try
            {
                _logger.LogInformation(
                    "Processing order queue message.");

                string messageBody = message.MessageText;

                Order? order =
                    JsonSerializer.Deserialize<Order>(
                        messageBody,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (order == null)
                {
                    _logger.LogError(
                        "Unable to deserialize order message.");

                    return;
                }

                // Initial status
                order.Status = "Received";

                // Save order to Azure Table
                await _orderTableService.AddOrderAsync(order);

                _logger.LogInformation(
                    "Order {OrderId} received and stored.",
                    order.OrderId);

                // Preparing
                await Task.Delay(2000);

                order.Status = "Preparing";

                await _orderTableService.UpdateOrderStatusAsync(
                    order.OrderTimestamp.UtcDateTime.ToString("yyyy-MM-dd"),
                    order.OrderId,
                    order.Status);

                _logger.LogInformation(
                    "Order {OrderId} status: Preparing",
                    order.OrderId);

                // Ready
                await Task.Delay(2000);

                order.Status = "Ready";

                await _orderTableService.UpdateOrderStatusAsync(
                    order.OrderTimestamp.UtcDateTime.ToString("yyyy-MM-dd"),
                    order.OrderId,
                    order.Status);

                _logger.LogInformation(
                    "Order {OrderId} status: Ready",
                    order.OrderId);

                // Collected
                await Task.Delay(2000);

                order.Status = "Collected";

                await _orderTableService.UpdateOrderStatusAsync(
                    order.OrderTimestamp.UtcDateTime.ToString("yyyy-MM-dd"),
                    order.OrderId,
                    order.Status);

                _logger.LogInformation(
                    "Order {OrderId} status: Collected",
                    order.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "ERROR PROCESSING ORDER: {Message}",
                    ex.Message);

                _logger.LogError(
                    "FULL ERROR: {Exception}",
                    ex.ToString());

                throw;
            }
        }
    }
}