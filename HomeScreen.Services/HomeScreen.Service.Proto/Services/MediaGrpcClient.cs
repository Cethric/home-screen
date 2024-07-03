using Grpc.Core;

namespace HomeScreen.Service.Proto.Services;

public class MediaGrpcClient : Service.Media.Media.MediaClient
{
    public MediaGrpcClient(ChannelBase channel) : base(channel)
    {
    }

    public MediaGrpcClient(CallInvoker callInvoker) : base(callInvoker)
    {
    }
}
