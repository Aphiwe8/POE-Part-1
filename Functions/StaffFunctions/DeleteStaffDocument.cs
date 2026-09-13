using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeNChill.Functions
{
    public class DeleteStaffDocument
    {
        [Function("DeleteStaffDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "delete",
                Route = "staff-documents/{fileName}")]
            HttpRequest req,
            string fileName)
        {
            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? throw new InvalidOperationException(
                    "AzureWebJobsStorage is missing.");

            BlobServiceClient blobServiceClient =
                new BlobServiceClient(connectionString);

            BlobContainerClient containerClient =
                blobServiceClient.GetBlobContainerClient("staff-docs");

            BlobClient blobClient =
                containerClient.GetBlobClient(fileName);

            bool deleted = await blobClient.DeleteIfExistsAsync();

            if (!deleted)
            {
                return new NotFoundObjectResult(new
                {
                    message = "Staff document not found.",
                    fileName = fileName
                });
            }

            return new OkObjectResult(new
            {
                message = "Staff document deleted successfully.",
                fileName = fileName
            });
        }
    }
}