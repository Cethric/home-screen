using Microsoft.EntityFrameworkCore;

namespace HomeScreen.Database.MediaDb;

public class MediaDbContext(DbContextOptions<MediaDbContext> options) : DbContext(options)
{
}
