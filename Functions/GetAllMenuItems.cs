using CoffeeAndChill.Services;
using CoffeeAndChill.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeAndChill.Functions
{
    public class GetAllMenuItems
    {
        private readonly MenuTableService _menuTableService;

        public GetAllMenuItems(MenuTableService menuTableService)
        {
            _menuTableService = menuTableService;
        }

        [Function("GetAllMenuItems")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "menu")]
            HttpRequest req)
        {
            List<MenuItem> menuItems =
                await _menuTableService.GetAllMenuItemsAsync();

            return new OkObjectResult(menuItems);
        }
    }
}