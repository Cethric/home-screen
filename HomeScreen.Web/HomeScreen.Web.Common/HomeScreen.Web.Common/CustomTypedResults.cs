using System.Text.Json;
using HomeScreen.Web.Common.JsonLines;

namespace HomeScreen.Web.Common;

public static class CustomTypedResults
{
    public static JsonLines<T> JsonLines<T>(
        IAsyncEnumerable<T> data,
        JsonSerializerOptions? jsonSerializerOptions = null
    ) =>
        new(data, jsonSerializerOptions);
}
