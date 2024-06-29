using Microsoft.Extensions.Hosting;

namespace HomeScreen.Database.MediaDb;

public static class MediaDbExtensions
{
    public static IHostApplicationBuilder AddMediaDb(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<MediaDbContext>("homescreen-media");

        return builder;
    }
}
