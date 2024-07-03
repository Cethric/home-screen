using System.Globalization;
using HomeScreen.Web.Slideshow.Server.Entities;
using Microsoft.AspNetCore.Http.Extensions;

namespace HomeScreen.Web.Slideshow.Server.Services;

public class MediaDownloader(ILogger<MediaDownloader> logger, IHttpClientFactory clientFactory) : IMediaDownloader
{
    public async Task<HttpResponseMessage> DownloadMedia(
        Guid id,
        long width,
        long height,
        float blur,
        MediaTransformOptionsFormat format,
        CancellationToken cancellationToken = default
    )
    {
        using var client = clientFactory.CreateClient("MediaDownloader");
        logger.LogInformation("Downloading media for {MediaId}", id);
        var builder = new UriBuilder("https", "homescreen-service-media", 7268);
        var query = new QueryBuilder
        {
            { "mediaId", id.ToString() },
            { "width", width.ToString() },
            { "height", height.ToString() },
            { "blurRadius", blur.ToString(CultureInfo.InvariantCulture) },
            { "format", format.ToString() }
        };
        builder.Path = "download";
        builder.Query = query.ToString();
        client.DefaultRequestVersion = new Version(2, 0);
        var response = await client.GetAsync(builder.ToString(), cancellationToken);
        return response;
    }
}
