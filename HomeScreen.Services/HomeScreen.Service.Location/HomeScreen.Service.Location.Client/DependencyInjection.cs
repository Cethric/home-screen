using Grpc.Health.V1;
using HomeScreen.Service.Location.Client.Infrastructure.Location;
using HomeScreen.Service.Location.Proto.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace HomeScreen.Service.Location.Client;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddLocationApi(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ILocationApi, LocationApi>();

        var isHttps = builder.Configuration["DOTNET_LAUNCH_PROFILE"] == "https";
        builder.Services.AddGrpcClient<LocationGrpcClient>(
            "homescreen-service-location",
            c =>
            {
                var url = $"{(isHttps ? "https" : "http")}://homescreen-service-location";
                c.Address = new Uri(url);
            }
        );

        return builder;
    }
}
