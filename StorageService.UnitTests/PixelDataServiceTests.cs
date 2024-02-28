using Moq;
using StorageService.FileWriter;
using StorageService.ServiceLayers;
using System.Text.RegularExpressions;

namespace StorageService.UnitTests
{
    public class PixelDataServiceTests
    {
        [Fact]
        public async Task SendPixelData_WritesLogEntryToFile_PopulatedData()
        {
            // Arrange
            var mockFileWriter = new Mock<IConcurrentFileWriter>();
            var storageGrpc = new PixelDataService(mockFileWriter.Object);
            var request = new PixelDataRequest
            {
                Referrer = "example.com",
                UserAgent = "Test UserAgent",
                IpAddress = "127.0.0.1"
            };

            string capturedFileEntry = null;
            mockFileWriter.Setup(
                writer => writer.WriteToFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            ).Callback<string, CancellationToken>((fileEntry, _) => capturedFileEntry = fileEntry);

            // Act
            await storageGrpc.SendPixelData(request, CancellationToken.None);

            // Assert
            var timestampRegex = new Regex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$");
            var timeStampAppendedInPayload = capturedFileEntry.Split(" | ")[0];
            Assert.Matches(timestampRegex, timeStampAppendedInPayload);
            Assert.Contains(request.Referrer, capturedFileEntry);
            Assert.Contains(request.UserAgent, capturedFileEntry);
            Assert.Contains(request.IpAddress, capturedFileEntry);
        }

        [Fact]
        public async Task SendPixelData_WritesLogEntryToFile_EmptyReferrer()
        {
            // Arrange
            var mockFileWriter = new Mock<IConcurrentFileWriter>();
            var storageGrpc = new PixelDataService(mockFileWriter.Object);
            var request = new PixelDataRequest
            {
                Referrer = "",
                UserAgent = "Test UserAgent",
                IpAddress = "127.0.0.1"
            };

            string expectedReferrer = "null";

            string capturedFileEntry = null;
            mockFileWriter.Setup(
                writer => writer.WriteToFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            ).Callback<string, CancellationToken>((fileEntry, _) => capturedFileEntry = fileEntry);

            // Act
            await storageGrpc.SendPixelData(request, CancellationToken.None);

            // Assert
            var timestampRegex = new Regex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$");
            var timeStampAppendedInPayload = capturedFileEntry.Split(" | ")[0];
            Assert.Matches(timestampRegex, timeStampAppendedInPayload);
            Assert.Contains(expectedReferrer, capturedFileEntry);
            Assert.Contains(request.UserAgent, capturedFileEntry);
            Assert.Contains(request.IpAddress, capturedFileEntry);
        }

        [Fact]
        public async Task SendPixelData_WritesLogEntryToFile_EmptyUserAgent()
        {
            // Arrange
            var mockFileWriter = new Mock<IConcurrentFileWriter>();
            var storageGrpc = new PixelDataService(mockFileWriter.Object);
            var request = new PixelDataRequest
            {
                Referrer = "example.com",
                UserAgent = "",
                IpAddress = "127.0.0.1"
            };

            string expectedReferrer = "null";

            string capturedFileEntry = null;
            mockFileWriter.Setup(
                writer => writer.WriteToFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            ).Callback<string, CancellationToken>((fileEntry, _) => capturedFileEntry = fileEntry);

            // Act
            await storageGrpc.SendPixelData(request, CancellationToken.None);

            // Assert
            var timestampRegex = new Regex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$");
            var timeStampAppendedInPayload = capturedFileEntry.Split(" | ")[0];
            Assert.Matches(timestampRegex, timeStampAppendedInPayload);
            Assert.Contains(expectedReferrer, capturedFileEntry);
            Assert.Contains(request.UserAgent, capturedFileEntry);
            Assert.Contains(request.IpAddress, capturedFileEntry);
        }

        [Theory]
        [InlineData("")]
        [InlineData("123/239")]
        public async Task SendPixelData_WritesLogEntryToFile_InvalidIp(string ip)
        {
            // Arrange
            var mockFileWriter = new Mock<IConcurrentFileWriter>();
            var storageGrpc = new PixelDataService(mockFileWriter.Object);
            var request = new PixelDataRequest
            {
                Referrer = "example.com",
                UserAgent = "Test UserAgent",
                IpAddress = ip
            };

            string capturedFileEntry = null;
            mockFileWriter.Setup(
                writer => writer.WriteToFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            ).Callback<string, CancellationToken>((fileEntry, _) => capturedFileEntry = fileEntry);

            // Act
            await storageGrpc.SendPixelData(request, CancellationToken.None);

            // Assert
            Assert.Null(capturedFileEntry);
        }
    }

}