using Xunit;
using Moq;
using static StorageServiceGrpc;
using StorageServiceGrpcLayer.UnitTests.Helpers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace StorageService.GrpcLayer.Tests
{

    public class PixelDataSenderTests
    {
        [Fact]
        public async Task SendDataAsync_ValidInput_CallsSendPixelDataAsync()
        {
            // Arrange
            var referrer = "http://example.com";
            var userAgent = "Mozilla/5.0";
            var ipAddress = "192.168.1.1";
            var mockCall = CallHelpers.CreateAsyncUnaryCall(new Empty());
            var mockClient = new Mock<StorageServiceGrpcClient>();
            mockClient.Setup(client => client.SendPixelDataAsync(It.IsAny<PixelDataRequest>(), null, null, CancellationToken.None)).
                Returns(mockCall);
            var sender = new PixelDataSender(mockClient.Object);

            // Act
            await sender.SendDataAsync(referrer, userAgent, ipAddress);

            // Assert
            mockClient.Verify(client => client.SendPixelDataAsync(It.IsAny<PixelDataRequest>(), null, null, CancellationToken.None), Times.Once);
        }

        [Fact]
        public void Constructor_NullStorageServiceClient_ThrowsArgumentNullException()
        {
            // Arrange, Act, Assert
            Assert.Throws<ArgumentNullException>(() => new PixelDataSender(null));
        }

        [Fact]
        public async Task SendDataAsync_NullIpAddress_DoesNotCallSendPixelDataAsync()
        {
            // Arrange
            var referrer = "http://example.com";
            var userAgent = "Mozilla/5.0";
            string ipAddress = null;
            var mockClient = new Mock<StorageServiceGrpcClient>();
            var sender = new PixelDataSender(mockClient.Object);

            // Act
            await sender.SendDataAsync(referrer, userAgent, ipAddress);

            // Assert
            mockClient.Verify(client => client.SendPixelDataAsync(It.IsAny<PixelDataRequest>(), null, null, CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task SendDataAsync_ExceptionThrown()
        {
            // Arrange
            var referrer = "http://example.com";
            var userAgent = "Mozilla/5.0";
            var ipAddress = "192.168.1.1";
            var mockClient = new Mock<StorageServiceGrpcClient>();
            var sender = new PixelDataSender(mockClient.Object);
            var exceptionMessage = "Test Exception Message";

            var mockCall = CallHelpers.CreateAsyncUnaryCall(StatusCode.Unavailable);
            mockClient.Setup(client => client.SendPixelDataAsync(It.IsAny<PixelDataRequest>(), null, null, CancellationToken.None))
                .Throws(new Exception(exceptionMessage));

            // Act
            await sender.SendDataAsync(referrer, userAgent, ipAddress);

            // Assert
            mockClient.Verify(client => client.SendPixelDataAsync(It.IsAny<PixelDataRequest>(), null, null, CancellationToken.None), Times.Once);
        }
    }
}
