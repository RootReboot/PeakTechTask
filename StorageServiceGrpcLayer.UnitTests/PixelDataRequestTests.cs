using Xunit;

namespace StorageServiceGrpcLayer.UnitTests
{
    public class PixelDataRequestTests
    {
        [Theory]
        [InlineData(null, null, null)]
        [InlineData("test", null, "test")]
        [InlineData("test", "test", null)]
        public async Task SetValues_WhenNullThrow(string referrer, string userAgent, string ipAddress)
        {
            // Arrange

            Assert.Throws<System.ArgumentNullException>(() =>
            {
                var request = new PixelDataRequest
                {
                    Referrer = referrer,
                    UserAgent = userAgent,
                    IpAddress = ipAddress
                };
            });
        }
    }
}
