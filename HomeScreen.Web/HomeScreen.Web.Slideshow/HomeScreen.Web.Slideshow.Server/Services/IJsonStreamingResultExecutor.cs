using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace HomeScreen.Web.Slideshow.Server.Services;

public interface IJsonStreamingResultExecutor<T> : IActionResultExecutor<JsonStreamingResult<T>>
{
}
