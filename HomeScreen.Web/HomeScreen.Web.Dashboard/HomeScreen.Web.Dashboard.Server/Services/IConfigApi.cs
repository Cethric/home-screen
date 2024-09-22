using HomeScreen.Web.Dashboard.Server.Entities;

namespace HomeScreen.Web.Dashboard.Server.Services;

public interface IConfigApi
{
    Task<Config?> ResolveConfig(CancellationToken cancellationToken = default);
}
