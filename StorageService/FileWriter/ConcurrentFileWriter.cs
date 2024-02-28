using System.Text;

namespace StorageService.FileWriter
{
    /// <summary>
    /// Provides methods for asynchronously writing content to a file in a thread-safe manner.
    /// </summary>
    public class ConcurrentFileWriter : IConcurrentFileWriter
    {
        private readonly string _filePath;
        private readonly SemaphoreSlim _semaphore;
        private bool _fileIsCreated;
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentFileWriter"/> class.
        /// </summary>
        /// <param name="filePath">The path to the file to write to.</param>
        public ConcurrentFileWriter(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _semaphore = new SemaphoreSlim(1);
        }

        /// <inheritdoc/>
        public async Task WriteToFileAsync(string content, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(content);

            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                EnsureFileExists();

                using var fileStream = new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
                var bytes = Encoding.UTF8.GetBytes(content);
                await fileStream.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Ensures that the directory and file exist, creating them if necessary.
        /// </summary>
        private void EnsureFileExists()
        {
            if(_fileIsCreated) return;

            // Get the directory path
            string directoryPath = Path.GetDirectoryName(_filePath);

            // Check if the directory exists, if not, create it
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Check if the file exists, if not, create it
            if (!File.Exists(_filePath))
            {
                // Create the file if it doesn't exist
                using var file = File.Create(_filePath);
            }

            _fileIsCreated = true;
        }
    }
}
