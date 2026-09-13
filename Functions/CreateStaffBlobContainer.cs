using CoffeeAndChill.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeNChill.Functions
{
    public class CreateStaffBlobContainer
    {
        private readonly BlobStorageService _blobStorageService;

        public CreateStaffBlobContainer(
            BlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        [Function("CreateStaffBlobContainer")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "setup/staff-docs")]
            HttpRequest req)
        {
            await _blobStorageService.CreateContainerAsync();

            return new OkObjectResult(new
            {
                message = "staff-docs blob container is ready."
            });
        }
    }
}