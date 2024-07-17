using System.Net;

namespace HomeScreen.Service.MediaClient
{
    namespace Generated
    {
        public partial class MediaFileClient
        {
            partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
            {
                request.Version = HttpVersion.Version20;
                request.VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
            }
        }
    }
}
