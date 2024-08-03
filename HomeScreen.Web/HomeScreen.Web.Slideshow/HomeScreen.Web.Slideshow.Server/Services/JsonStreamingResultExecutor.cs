using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace HomeScreen.Web.Slideshow.Server.Services;

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
    public async Task ExecuteAsync(ActionContext context, JsonStreamingResult<T> result)
{
    logger.LogInformation("Executing JsonStreamingResult");
    var response = context.HttpContext.Response;
    response.StatusCode = (int)HttpStatusCode.OK;
    response.ContentType = $"{MediaTypeNames.Application.JsonSequence};charset={Encoding.UTF8.WebName}";

    await foreach (var value in result.Data)
    {
        context.HttpContext.RequestAborted.ThrowIfCancellationRequested();
        logger.LogInformation("Progressing JsonStreamingResult");
        await JsonSerializer.SerializeAsync(
            response.Body,
            value,
            result.JsonSerializerOptions,
            context.HttpContext.RequestAborted
        );
        await response.BodyWriter.WriteAsync(Line, context.HttpContext.RequestAborted);
        await response.BodyWriter.FlushAsync(context.HttpContext.RequestAborted);
    }

    logger.LogInformation("Executed JsonStreamingResult");
}
}
