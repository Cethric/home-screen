using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace HomeScreen.Web.Common;

public class JsonStreamingResult<TValue>(
    IAsyncEnumerable<TValue> data,
    JsonSerializerOptions? jsonSerializerOptions = null
) : IActionResult, IResult, IEndpointMetadataProvider, IStatusCodeHttpResult
{
    private static ActivitySource ActivitySource => new(nameof(JsonStreamingResult<TValue>));
    public IAsyncEnumerable<TValue> Data { get; } = data;
    public JsonSerializerOptions? JsonSerializerOptions { get; } = jsonSerializerOptions;

    public async Task ExecuteResultAsync(ActionContext context)
    {
        using var activity = ActivitySource.StartActivity("ExecuteResultAsync", ActivityKind.Client);
        var executor = context.HttpContext.RequestServices.GetRequiredService<IJsonStreamingResultExecutor<TValue>>();

        await executor.ExecuteAsync(context, this);
    }

    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        using var activity = ActivitySource.StartActivity("PopulateMetadata", ActivityKind.Client);
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(builder);

        builder.Metadata.Add(
            new ProducesResponseTypeMetadata(StatusCodes.Status200OK, typeof(TValue), new[] { "application/json" })
        );
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        using var activity = ActivitySource.StartActivity("ExecuteAsync", ActivityKind.Client);
        var executor = httpContext.RequestServices.GetRequiredService<IJsonStreamingResultExecutor<TValue>>();

        await executor.ExecuteAsync(httpContext, this);
    }

    public int? StatusCode => StatusCodes.Status200OK;
}
