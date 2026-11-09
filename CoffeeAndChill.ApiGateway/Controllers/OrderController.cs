using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeAndChill.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Submits an order to the order processing queue.
        /// </summary>
        /// <summary>
        /// Submits an order to the order processing queue.
        /// </summary>
        [HttpPost("queue")]
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> QueueOrder(
            [FromBody] System.Text.Json.JsonElement order)
        {
            var client = _httpClientFactory.CreateClient("Functions");

            var response = await client.PostAsJsonAsync(
                "api/orders/queue",
                order
            );

            var content = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = content,
                ContentType = response.Content.Headers.ContentType?.ToString()
                    ?? "application/json",
                StatusCode = (int)response.StatusCode
            };
        }

        /// <summary>
        /// Retrieves the status and details of an order.
        /// </summary>
        [HttpGet("{orderDate}/{orderId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrderStatus(
            string orderDate,
            string orderId)
        {
            var client = _httpClientFactory.CreateClient("Functions");

            var response = await client.GetAsync(
                $"api/orders/{Uri.EscapeDataString(orderDate)}/{Uri.EscapeDataString(orderId)}"
            );

            var content = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = content,
                ContentType = response.Content.Headers.ContentType?.ToString()
                    ?? "application/json",
                StatusCode = (int)response.StatusCode
            };
        }
    }
}
