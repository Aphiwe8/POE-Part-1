using Azure;
using CoffeeAndChill.Models;
using CoffeeAndChill.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;

namespace CoffeeNChill.Functions
{
    public class UpdateMenuItem
    {
        private readonly MenuTableService _menuTableService;

        public UpdateMenuItem(MenuTableService menuTableService)
        {
            _menuTableService = menuTableService;
        }

        [Function("UpdateMenuItem")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "put",
                Route = "menu/{category}/{id}")]
            HttpRequest req,
            string category,
            string id)
        {
            if (string.IsNullOrWhiteSpace(category) ||
                string.IsNullOrWhiteSpace(id))
            {
                return new BadRequestObjectResult(new
                {
                    error = "Category and ID are required."
                });
            }

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

            menuItem.Category = category;
            menuItem.Id = id;

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

            try
            {
                await _menuTableService.UpdateMenuItemAsync(menuItem);
            }
            catch (RequestFailedException ex)
                when (ex.Status == 404)
            {
                return new NotFoundObjectResult(new
                {
                    error = "Menu item not found."
                });
            }

            return new OkObjectResult(new
            {
                message = "Menu item updated successfully.",
                item = menuItem
            });
        }
    }
}