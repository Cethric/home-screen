using Grpc.Core;

namespace HomeScreen.Web.Slideshow.Server.Infrastructure.Services;

public class WeatherGrpcClient : Service.Weather.Weather.WeatherClient
{
    public WeatherGrpcClient(ChannelBase channel) : base(channel)
    {
    }

    public WeatherGrpcClient(CallInvoker callInvoker) : base(callInvoker)
    {
    }
}
