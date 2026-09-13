using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeAndChill.Functions.StaffFunctions
{
    public class UploadStaffDocument
    {
        [Function("UploadStaffDocument")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "staff-documents/upload")]
            HttpRequest req)
        {
            if (req.Form.Files.Count == 0)
            {
                return new BadRequestObjectResult(new
                {
                    message = "Please upload a file."
                });
            }

            var file = req.Form.Files[0];

            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? throw new InvalidOperationException(
                    "AzureWebJobsStorage is missing.");

            BlobServiceClient blobServiceClient =
                new BlobServiceClient(connectionString);

            BlobContainerClient containerClient =
                blobServiceClient.GetBlobContainerClient("staff-docs");

            BlobClient blobClient =
                containerClient.GetBlobClient(file.FileName);

            using Stream stream = file.OpenReadStream();

            await blobClient.UploadAsync(
                stream,
                overwrite: true);

            return new OkObjectResult(new
            {
                message = "Staff document uploaded successfully.",
                fileName = file.FileName,
                container = "staff-docs"
            });
        }
    }
}