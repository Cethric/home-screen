using Microsoft.Extensions.Hosting;

namespace HomeScreen.Database.MediaDb;

public static class MediaDbExtensions
{
    public static IHostApplicationBuilder AddMediaDb(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<MediaDbContext>("homescreen-mediadb");

        return builder;
    }
}
