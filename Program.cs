using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using CoffeeAndChill.Services;



var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();


builder.Services.AddSingleton<MenuTableService>();
builder.Services.AddSingleton<StaffDocumentService>();
builder.Services.AddSingleton<BlobStorageService>();

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("UseDevelopmentStorage=true")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

builder.Build().Run();
