using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeAndChill;

public class CoffeeAndChillTest
{
    private readonly ILogger<CoffeeAndChillTest> _logger;

    public CoffeeAndChillTest(ILogger<CoffeeAndChillTest> logger)
    {
        _logger = logger;
    }

    [Function("CoffeeAndChillTest")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("CoffeeAndChillTest is Running");
    }
}

//COmpose.yaml