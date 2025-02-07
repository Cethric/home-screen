using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HomeScreen.Web.Common.JsonLines;

public class JsonLinesExecutor
{
    protected static readonly ReadOnlyMemory<byte> Line = new([(byte)'\n']);

    protected JsonLinesExecutor()
    {
    }
}

public class JsonLinesExecutor<TValue>(ILogger<JsonLinesExecutor<TValue>> logger)
    : JsonLinesExecutor, IJsonLinesExecutor<TValue>
{
    private static ActivitySource ActivitySource => new(nameof(JsonLines<TValue>));

    public async Task ExecuteAsync(HttpContext httpContext, JsonLines<TValue> jsonLines)
    {
        using var activity = ActivitySource.StartActivity();
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(jsonLines);

        logger.LogTrace("Executing JsonLines");
        var response = httpContext.Response;
        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentType = $"{MediaTypeNames.Application.JsonSequence};charset={Encoding.UTF8.WebName}";

        await foreach (var value in jsonLines.Data)
            await ProgressLine(httpContext, value, response, jsonLines.JsonSerializerOptions);

        logger.LogTrace("Executed JsonLines");
    }

    private async Task ProgressLine(
        HttpContext httpContext,
        TValue value,
        HttpResponse response,
        JsonSerializerOptions? serializerOptions
    )
    {
        using var activity = ActivitySource.StartActivity();
        httpContext.RequestAborted.ThrowIfCancellationRequested();
        logger.LogTrace("Progressing JsonLines");
        await JsonSerializer.SerializeAsync(response.Body, value, serializerOptions, httpContext.RequestAborted);
        await response.Body.WriteAsync(Line, httpContext.RequestAborted);
        await response.Body.FlushAsync(httpContext.RequestAborted);
    }
}
