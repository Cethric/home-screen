using System.Text.Json;

namespace HomeScreen.Web.Common;

public static class CustomTypedResults
{
    public static JsonStreamingResult<T> JsonStreaming<T>(
        IAsyncEnumerable<T> data,
        JsonSerializerOptions? jsonSerializerOptions = null
    ) =>
        new(data, jsonSerializerOptions);
}
