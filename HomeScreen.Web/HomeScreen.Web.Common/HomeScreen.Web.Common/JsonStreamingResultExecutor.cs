using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HomeScreen.Web.Common;

public class JsonStreamingResultExecutor
{
    protected static readonly ReadOnlyMemory<byte> Line = new([(byte)'\n']);

    protected JsonStreamingResultExecutor()
    {
    }
}

public class JsonStreamingResultExecutor<T>(ILogger<JsonStreamingResult<T>> logger)
    : JsonStreamingResultExecutor, IJsonStreamingResultExecutor<T>
{
    private static ActivitySource ActivitySource => new(nameof(JsonStreamingResultExecutor<T>));

public async Task ExecuteAsync(ActionContext context, JsonStreamingResult<T> result)
{
    using var activity = ActivitySource.StartActivity("ExecuteAsync", ActivityKind.Client);
    await ExecuteAsync(context.HttpContext, result);
}

public async Task ExecuteAsync(HttpContext httpContext, JsonStreamingResult<T> result)
{
    using var activity = ActivitySource.StartActivity("ExecuteAsync", ActivityKind.Client);
    logger.LogInformation("Executing JsonStreamingResult");
    var response = httpContext.Response;
    response.StatusCode = (int)HttpStatusCode.OK;
    response.ContentType = $"{MediaTypeNames.Application.JsonSequence};charset={Encoding.UTF8.WebName}";

    await foreach (var value in result.Data)
    {
        httpContext.RequestAborted.ThrowIfCancellationRequested();
        logger.LogInformation("Progressing JsonStreamingResult");
        await JsonSerializer.SerializeAsync(
            response.Body,
            value,
            result.JsonSerializerOptions,
            httpContext.RequestAborted
        );
        await response.BodyWriter.WriteAsync(Line, httpContext.RequestAborted);
        await response.BodyWriter.FlushAsync(httpContext.RequestAborted);
    }

    logger.LogInformation("Executed JsonStreamingResult");
}
}
