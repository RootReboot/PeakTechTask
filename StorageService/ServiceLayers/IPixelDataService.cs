namespace StorageService.ServiceLayers
{
    /// <summary>
    /// Interface defining the contract for a service responsible for processing and logging pixel data.
    /// </summary>
    public interface IPixelDataService
    {
        /// <summary>
        /// Sends pixel data for logging.
        /// </summary>
        /// <param name="request">The pixel data request to be processed and logged.</param>
        /// <param name="cancellationToken">Token that receives a notification in case the operation is to be halted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        ValueTask SendPixelData(PixelDataRequest request, CancellationToken cancellationToken);
    }
}
