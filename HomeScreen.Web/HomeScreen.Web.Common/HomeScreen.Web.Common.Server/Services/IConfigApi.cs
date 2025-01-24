using HomeScreen.Web.Common.Server.Entities;

namespace HomeScreen.Web.Common.Server.Services;

public interface IConfigApi
{
    Task<Config> ResolveConfig(CancellationToken cancellationToken = default);
}
