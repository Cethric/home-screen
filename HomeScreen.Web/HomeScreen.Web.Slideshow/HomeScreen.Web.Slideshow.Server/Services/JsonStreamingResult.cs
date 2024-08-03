using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace HomeScreen.Web.Slideshow.Server.Services;

public class JsonStreamingResult<T>(IAsyncEnumerable<T> data, JsonSerializerOptions? jsonSerializerOptions = null)
    : IActionResult
{
    public IAsyncEnumerable<T> Data { get; } = data;
public JsonSerializerOptions? JsonSerializerOptions { get; } = jsonSerializerOptions;

public async Task ExecuteResultAsync(ActionContext context)
{
    var executor = context.HttpContext.RequestServices.GetRequiredService<IJsonStreamingResultExecutor<T>>();

    await executor.ExecuteAsync(context, this);
}
}
