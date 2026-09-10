using CoffeeAndChill.Models;
using CoffeeAndChill.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeNChill.Functions
{
    public class GetMenuItemsByCategory
    {
        private readonly MenuTableService _menuTableService;

        public GetMenuItemsByCategory(MenuTableService menuTableService)
        {
            _menuTableService = menuTableService;
        }

        [Function("GetMenuItemsByCategory")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "menu/category/{category}")]
            HttpRequest req,
            string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return new BadRequestObjectResult(new
                {
                    error = "Category is required."
                });
            }

            List<MenuItem> menuItems =
                await _menuTableService.GetMenuItemsByCategoryAsync(category);

            return new OkObjectResult(menuItems);
        }
    }
}