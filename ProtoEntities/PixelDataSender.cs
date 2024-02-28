using static StorageServiceGrpc;

namespace StorageService.GrpcLayer
{
    /// <summary>
    /// Provides functionality to send pixel data asynchronously via gRPC.
    /// </summary>
    public class PixelDataSender : IPixelDataSender
    {
        private readonly StorageServiceGrpcClient _storageServiceClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="PixelDataSender"/> class.
        /// </summary>
        /// <param name="storagePixelGrpcClient">The gRPC client for storage service.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="storagePixelGrpcClient"/> is null.</exception>
        public PixelDataSender(StorageServiceGrpcClient storagePixelGrpcClient)
        {
            _storageServiceClient = storagePixelGrpcClient ?? throw new ArgumentNullException(nameof(storagePixelGrpcClient));
        }

        /// <summary>
        /// Sends pixel data asynchronously.
        /// </summary>
        /// <param name="referrer">The referrer URL.</param>
        /// <param name="userAgent">The user agent string.</param>
        /// <param name="ipAddress">The IP address.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method sends pixel data asynchronously to the storage service using gRPC.
        /// </remarks>
        public async ValueTask SendDataAsync(string referrer, string userAgent, string ipAddress)
        {
            if (ipAddress == null)
                return;

            var request = new PixelDataRequest()
            {
                Referrer = referrer,
                UserAgent = userAgent,
                IpAddress = ipAddress
            };

            try
            {
                await _storageServiceClient.SendPixelDataAsync(request);
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or perform any necessary action
                Console.WriteLine($"An error occurred while sending pixel data: {ex.Message}");
            }
        }
    }
}
