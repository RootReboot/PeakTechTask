using StorageService.FileWriter;
using System.Net;

namespace StorageService.ServiceLayers
{
    /// <summary>
    /// Implementation of the IPixelDataService interface responsible for processing and logging pixel data.
    /// </summary>
    public class PixelDataService : IPixelDataService
    {
        private readonly IConcurrentFileWriter _concurrentFileWriter;

        /// <summary>
        /// Initializes a new instance of the PixelDataService class.
        /// </summary>
        /// <param name="concurrentFileWriter">The file writer used for writing the log entry asynchronously.</param>
        public PixelDataService(IConcurrentFileWriter concurrentFileWriter)
        {
            _concurrentFileWriter = concurrentFileWriter;
        }

        /// <inheritdoc/>
        public async ValueTask SendPixelData(PixelDataRequest request, CancellationToken cancellationToken)
        {
            //TODO: Is there a need to differentiate ipv4 from ipv6
            if (!IPAddress.TryParse(request.IpAddress, out var _))
            {
                //Log
                return;
            }

            var logEntry = FormatLogEntry(request);
            await _concurrentFileWriter.WriteToFileAsync(logEntry, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Formats the log entry based on the pixel data request.
        /// </summary>
        /// <param name="request">The pixel data request to be formatted.</param>
        /// <returns>The formatted log entry.</returns>
        private string FormatLogEntry(PixelDataRequest request)
        {
            var timestamp = DateTime.UtcNow.ToString("s");

            var referrer = string.IsNullOrEmpty(request.Referrer) ? "null" : request.Referrer;
            var userAgent = string.IsNullOrEmpty(request.UserAgent) ? "null" : request.UserAgent;
            return $"{timestamp} | {referrer} | {userAgent} | {request.IpAddress}\n";
        }
    }
}
