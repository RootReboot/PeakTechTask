using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using StorageService.ServiceLayers;

namespace StorageService.Services
{
    public sealed class StorageGrpc : StorageServiceGrpc.StorageServiceGrpcBase
    {
        private readonly IPixelDataService _pixelDataService;
        //Reduce allocations.
        private readonly Empty _reuseEmpty;

        public StorageGrpc(IPixelDataService pixelDataService) : base()
        {
            _pixelDataService = pixelDataService;
            _reuseEmpty = new Empty();
        }

        public override async Task<Empty> SendPixelData(PixelDataRequest request, ServerCallContext context)
        {
            await _pixelDataService.SendPixelData(request, context.CancellationToken);

            return _reuseEmpty;
        }
    }
}
