using CoffeeAndChill.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeAndChill.Functions
{
    public class CreateMenuTable
    {
        private readonly MenuTableService _menuTableService;

        public CreateMenuTable(MenuTableService menuTableService)
        {
            _menuTableService = menuTableService;
        }

        [Function("CreateMenuTable")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "setup/menu-table")]
            HttpRequest req)
        {
            await _menuTableService.CreateTableAsync();

            return new OkObjectResult(new
            {
                message = "MenuItems table is ready."
            });
        }
    }
}