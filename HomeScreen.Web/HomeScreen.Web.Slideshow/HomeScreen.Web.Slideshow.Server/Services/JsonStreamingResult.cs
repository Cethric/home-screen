using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace HomeScreen.Web.Slideshow.Server.Services;

public class JsonStreamingResult<TValue>(
    IAsyncEnumerable<TValue> data,
    JsonSerializerOptions? jsonSerializerOptions = null
) : IActionResult, IResult, IEndpointMetadataProvider, IStatusCodeHttpResult
{
    public IAsyncEnumerable<TValue> Data { get; } = data;
public JsonSerializerOptions? JsonSerializerOptions { get; } = jsonSerializerOptions;

public async Task ExecuteResultAsync(ActionContext context)
{
    var executor = context.HttpContext.RequestServices.GetRequiredService<IJsonStreamingResultExecutor<TValue>>();

    await executor.ExecuteAsync(context, this);
}

public async Task ExecuteAsync(HttpContext httpContext)
{
    var executor = httpContext.RequestServices.GetRequiredService<IJsonStreamingResultExecutor<TValue>>();

    await executor.ExecuteAsync(httpContext, this);
}

public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
{
    ArgumentNullException.ThrowIfNull(method);
    ArgumentNullException.ThrowIfNull(builder);

    builder.Metadata.Add(
        new ProducesResponseTypeMetadata(StatusCodes.Status200OK, typeof(TValue), new[] { "application/json" })
    );
}

public int? StatusCode => StatusCodes.Status200OK;
}
