using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HomeScreen.Database.MediaDb.Contexts;

public class MediaDbDesignTimeContextFactory : IDesignTimeDbContextFactory<MediaDbContext>
{
    public MediaDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<MediaDbContext>();
        builder.UseSqlServer();
        return new MediaDbContext(builder.Options);
    }
}
