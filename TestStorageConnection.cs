using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeAndChill.Functions
{
    public class TestStorageConnection
    {
        [Function("TestStorageConnection")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "test-storage")]
            HttpRequest req)
        {
            //this line here basically finds the connection string in the local.settings.json file and uses it to connect to the Azure Table Storage
            //answers the "what storage connection has my application been configured to use?"
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            //creates the client that will be used to connect to the Azure Table Storage
            TableServiceClient serviceClient =
                new TableServiceClient(connectionString);

            string tableName = "TestTable";

            //basically sais create table if it doesn't exist, if it does exist, do nothing 

            await serviceClient.CreateTableIfNotExistsAsync(tableName);
             
            return new OkObjectResult(new
            {
                message = "Successfully connected to Azure Table Storage!",
                table = tableName
            });
        }
    }
}