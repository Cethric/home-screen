using Microsoft.AspNetCore.Http;

namespace HomeScreen.Web.Common.JsonLines;

public interface IJsonLinesExecutor<TValue>
{
    Task ExecuteAsync(HttpContext httpContext, JsonLines<TValue> jsonLines);
}
