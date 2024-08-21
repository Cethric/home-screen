using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace HomeScreen.Web.Common;

public interface IJsonStreamingResultExecutor<T> : IActionResultExecutor<JsonStreamingResult<T>>
{
    Task ExecuteAsync(HttpContext httpContext, JsonStreamingResult<T> result);
}
