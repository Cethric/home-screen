using System.Diagnostics;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace HomeScreen.Web.Common.JsonLines;

public class JsonLines<TValue>(IAsyncEnumerable<TValue> data, JsonSerializerOptions? jsonSerializerOptions = null)
    : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult
{
    private static ActivitySource ActivitySource => new(nameof(JsonLines<TValue>));
public IAsyncEnumerable<TValue> Data { get; } = data;
public JsonSerializerOptions? JsonSerializerOptions { get; } = jsonSerializerOptions;

public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
{
    using var activity = ActivitySource.StartActivity();
    ArgumentNullException.ThrowIfNull(method);
    ArgumentNullException.ThrowIfNull(builder);

    builder.Metadata.Add(
        new ProducesResponseTypeMetadata(
            StatusCodes.Status200OK,
            typeof(TValue),

            [$"{MediaTypeNames.Application.JsonSequence};charset={Encoding.UTF8.WebName}"]
        )
    );
}

public async Task ExecuteAsync(HttpContext httpContext)
{
    using var activity = ActivitySource.StartActivity();
    ArgumentNullException.ThrowIfNull(httpContext);
    var executor = httpContext.RequestServices.GetRequiredService<IJsonLinesExecutor<TValue>>();
    await executor.ExecuteAsync(httpContext, this);
}

public int? StatusCode => StatusCodes.Status200OK;
}
