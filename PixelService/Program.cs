using Aspose.Drawing;
using StorageService.GrpcLayer;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Aspose.Drawing.Imaging;
var builder = WebApplication.CreateBuilder(args);

// Parse command line arguments to get the port and host
var port = args.Length > 0 ? Convert.ToInt32(args[0]) : 5002; // Default to port 5000 if no argument is provided
var host = args.Length > 1 ? args[1] : "localhost"; // Default to localhost if no argument is provided

// Add configuration for reading settings from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddSingleton<IPixelDataSender>(sp =>
{
    var defaultMethodConfig = new MethodConfig
    {
        Names = { MethodName.Default },
        RetryPolicy = new RetryPolicy
        {
            MaxAttempts = 5,
            InitialBackoff = TimeSpan.FromSeconds(1),
            MaxBackoff = TimeSpan.FromSeconds(5),
            BackoffMultiplier = 1.5,
            RetryableStatusCodes = { StatusCode.Unavailable }
        }
    };

    var storageGrpcEndpoint = builder.Configuration.GetValue<string>("StorageGrpcEndpoint");
    var channel = GrpcChannel.ForAddress(storageGrpcEndpoint);
    var grpcClient = new StorageServiceGrpc.StorageServiceGrpcClient(channel);
    return new PixelDataSender(grpcClient);
});


var app = builder.Build();

// Define the endpoint for tracking
app.MapGet("/track", static async (HttpContext context,
    IPixelDataSender dataSenderService) =>
{
    // Set response headers
    context.Response.ContentType = "image/gif";

    //This can be cached.
    using var image = new Bitmap(1, 1, PixelFormat.Format32bppArgb);

    // Save the image to a MemoryStream
    using (var memStream = new MemoryStream())
    {
        image.Save(memStream, ImageFormat.Gif);
        memStream.Seek(0, SeekOrigin.Begin);

        // Copy the contents of the MemoryStream to the response body
        await memStream.CopyToAsync(context.Response.Body).ConfigureAwait(false);
    }

    // Collect information asynchronously
    var referrer = context.Request.Headers.Referer.ToString();
    var userAgent = context.Request.Headers.UserAgent.ToString();
    var ipAddress = context.Connection?.RemoteIpAddress?.ToString();

    await dataSenderService.SendDataAsync(null, userAgent, ipAddress).ConfigureAwait(false);
});


app.Run($"http://{host}:{port}");
