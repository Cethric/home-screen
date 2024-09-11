using HomeScreen.Web.Slideshow.Server.Entities;

namespace HomeScreen.Web.Slideshow.Server.Services;

public interface IConfigApi
{
    Task<Config?> ResolveConfig(CancellationToken cancellationToken = default);
}
