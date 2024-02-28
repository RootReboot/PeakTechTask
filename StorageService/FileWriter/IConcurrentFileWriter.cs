namespace StorageService.FileWriter
{
    /// <summary>
    /// Defines methods for asynchronously writing content to a file in a thread-safe manner.
    /// </summary>
    public interface IConcurrentFileWriter
    {
        /// <summary>
        /// Writes the specified content to the file asynchronously.
        /// </summary>
        /// <param name="content">The content to write to the file.</param>
        /// <param name="cancellationToken">Token that receives a notification in case the operation is to be halted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task WriteToFileAsync(string content, CancellationToken cancellationToken);
    }
}
