using Grpc.Core;

namespace HomeScreen.Service.Media.Proto.Services;

public class MediaGrpcClient : Media.MediaClient
{
    public MediaGrpcClient(ChannelBase channel) : base(channel)
    {
    }

    public MediaGrpcClient(CallInvoker callInvoker) : base(callInvoker)
    {
    }
}
