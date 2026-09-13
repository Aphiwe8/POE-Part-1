using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeAndChill.Functions.StaffFunctions
{
    public class DownloadStaffDocument
    {
        [Function("DownloadStaffDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
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

            if (!await blobClient.ExistsAsync())
            {
                return new NotFoundObjectResult(new
                {
                    message = "Staff document not found.",
                    fileName = fileName
                });
            }

            var response = await blobClient.DownloadStreamingAsync();

            return new FileStreamResult(
                response.Value.Content,
                response.Value.Details.ContentType ?? "application/octet-stream")
            {
                FileDownloadName = fileName
            };
        }
    }
}