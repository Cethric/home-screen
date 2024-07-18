namespace HomeScreen.Service.Media.Infrastructure.Location;

public class NominatimResponse
{
    public string display_name { get; set; }
}

public class NominatimLocationService(ILogger<NominatimLocationService> logger, IHttpClientFactory httpClientFactory)
    : ILocationService
{
    public async Task<string> SearchForLocation(
        double longitude,
        double latitude,
        double altitude,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Attempting to search for address at {Longitude}, {Latitude}, {Altitude}",
            longitude,
            latitude,
            altitude
        );

        using var client = httpClientFactory.CreateClient("Nominatim");
        client.BaseAddress = new Uri("https://nominatim.geocoding.ai/");

        var builder = new UriBuilder(client.BaseAddress)
        {
            Path = "/reverse.php", Query = $"lat={latitude}&lon={longitude}&zoom=13&format=jsonv2"
        };
        var response = await client.GetFromJsonAsync<NominatimResponse>(builder.Uri, cancellationToken);
        return response is not null ? response.display_name : "Unknown location";
    }
}
