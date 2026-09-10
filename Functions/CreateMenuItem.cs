using CoffeeAndChill.Models;
using CoffeeAndChill.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;

namespace CoffeeNChill.Functions
{
    public class CreateMenuItem
    {
        private readonly MenuTableService _menuTableService;

        public CreateMenuItem(MenuTableService menuTableService)
        {
            _menuTableService = menuTableService;
        }

        [Function("CreateMenuItem")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "menu")]
            HttpRequest req)
        {
            string requestBody =
                await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(requestBody))
            {
                return new BadRequestObjectResult(new
                {
                    error = "Request body cannot be empty."
                });
            }

            MenuItem? menuItem;

            try
            {
                menuItem = JsonSerializer.Deserialize<MenuItem>(
                    requestBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch (JsonException)
            {
                return new BadRequestObjectResult(new
                {
                    error = "Invalid JSON format."
                });
            }

            if (menuItem == null)
            {
                return new BadRequestObjectResult(new
                {
                    error = "Invalid menu item."
                });
            }

            if (string.IsNullOrWhiteSpace(menuItem.Category))
            {
                return new BadRequestObjectResult(new
                {
                    error = "Category is required."
                });
            }

            if (string.IsNullOrWhiteSpace(menuItem.Id))
            {
                return new BadRequestObjectResult(new
                {
                    error = "ID is required."
                });
            }

            if (string.IsNullOrWhiteSpace(menuItem.Name))
            {
                return new BadRequestObjectResult(new
                {
                    error = "Name is required."
                });
            }

            if (menuItem.Price <= 0)
            {
                return new BadRequestObjectResult(new
                {
                    error = "Price must be greater than zero."
                });
            }

            await _menuTableService.CreateMenuItemAsync(menuItem);

            return new ObjectResult(new
            {
                message = "Menu item created successfully.",
                item = menuItem
            })
            {
                StatusCode = StatusCodes.Status201Created
            };
        }
    }
}