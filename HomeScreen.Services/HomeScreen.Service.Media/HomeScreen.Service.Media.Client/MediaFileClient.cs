using System.Net;

namespace HomeScreen.Service.Media.Client
{
    namespace Generated
    {
        public partial class MediaClient
        {
            partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
            {
                ArgumentNullException.ThrowIfNull(client);
                ArgumentNullException.ThrowIfNull(request);
                ArgumentNullException.ThrowIfNullOrEmpty(url);

                request.Version = HttpVersion.Version20;
                request.VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
            }
        }
    }
}
