using CoffeeAndChill.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeAndChill.Functions
{
    public class CreateStaffDocumentShare
    {
        private readonly StaffDocumentService _staffDocumentService;

        public CreateStaffDocumentShare(
            StaffDocumentService staffDocumentService)
        {
            _staffDocumentService = staffDocumentService;
        }

        [Function("CreateStaffDocumentShare")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "setup/staff-docs")]
            HttpRequest req)
        {
            await _staffDocumentService.CreateShareAsync();

            return new OkObjectResult(new
            {
                message = "staff-docs file share is ready."
            });
        }
    }
}