using Grpc.Core;

namespace HomeScreen.Service.Weather.Proto.Services;

public class WeatherGrpcClient : Weather.WeatherClient
{
    public WeatherGrpcClient(ChannelBase channel) : base(channel)
    {
    }

    public WeatherGrpcClient(CallInvoker callInvoker) : base(callInvoker)
    {
    }
}
