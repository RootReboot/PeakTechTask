using StorageService.Services;
using StorageService.FileWriter;
using StorageService.ServiceLayers;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // Setup a HTTP/2 endpoint without TLS.
    options.ListenLocalhost(5000, o => o.Protocols =
        HttpProtocols.Http2);
});

builder.Configuration.AddJsonFile("appsettings.json");

// Add services to the container.
builder.Services.AddGrpc();

// Register your dependencies
builder.Services.AddSingleton<IConcurrentFileWriter, ConcurrentFileWriter>((serviceProvider) =>
{
    var filePath = builder.Configuration.GetValue<string>("FilePath");
    return new ConcurrentFileWriter(filePath);
});

builder.Services.AddSingleton<IPixelDataService, PixelDataService>((serviceProvider) =>
{
    var fileWriter = serviceProvider.GetRequiredService<IConcurrentFileWriter>();
    return new PixelDataService(fileWriter);
});

var app = builder.Build();

app.MapGrpcService<StorageGrpc>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();