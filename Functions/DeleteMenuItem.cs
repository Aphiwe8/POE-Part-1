using Azure;
using CoffeeAndChill.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeNChill.Functions
{
    public class DeleteMenuItem
    {
        private readonly MenuTableService _menuTableService;

        public DeleteMenuItem(MenuTableService menuTableService)
        {
            _menuTableService = menuTableService;
        }

        [Function("DeleteMenuItem")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "delete",
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

            try
            {
                await _menuTableService.DeleteMenuItemAsync(
                    category,
                    id);
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
                message = "Menu item deleted successfully."
            });
        }
    }
}