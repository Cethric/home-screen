using Grpc.Core;

namespace HomeScreen.Service.Location.Proto.Services;

public class LocationGrpcClient : Location.LocationClient
{
    public LocationGrpcClient(ChannelBase channel) : base(channel)
    {
    }

    public LocationGrpcClient(CallInvoker callInvoker) : base(callInvoker)
    {
    }
}
