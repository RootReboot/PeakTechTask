namespace StorageService.GrpcLayer
{
    /// <summary>
    /// Represents functionality for sending pixel data asynchronously.
    /// </summary>
    public interface IPixelDataSender
    {
        /// <summary>
        /// Sends pixel data asynchronously.
        /// </summary>
        /// <param name="referrer">The referrer URL.</param>
        /// <param name="userAgent">The user agent string.</param>
        /// <param name="ipAddress">The IP address.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// Implementations of this method should send pixel data asynchronously to a storage service.
        /// </remarks>
        ValueTask SendDataAsync(string referrer, string userAgent, string ipAddress);
    }
}
