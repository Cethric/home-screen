using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace HomeScreen.Web.Slideshow.Server.Services;

public class JsonStreamingResultExecutor
{
    protected static readonly ReadOnlyMemory<byte> Line = new([(byte)'\n']);
}

public class JsonStreamingResultExecutor<T> : JsonStreamingResultExecutor, IJsonStreamingResultExecutor<T>
{
    public async Task ExecuteAsync(ActionContext context, JsonStreamingResult<T> result)
    {
        HttpResponse response = context.HttpContext.Response;
        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentType = $"{MediaTypeNames.Application.JsonSequence};charset={Encoding.UTF8.WebName}";

        await foreach (var value in result.Data)
        {
            await using var stream = response.BodyWriter.AsStream();
            await JsonSerializer.SerializeAsync(
                stream,
                value,
                result.JsonSerializerOptions,
                context.HttpContext.RequestAborted
            );
            await response.BodyWriter.WriteAsync(Line, context.HttpContext.RequestAborted);
        }
    }
}
