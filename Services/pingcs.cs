using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

//temporary class to test app functionality in linux,docker andazure
public class Ping
{
    [Function("Ping")]
    public async Task<HttpResponseData> Run(
     [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ping")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync("pong");
        return response;
    }
}