using HomeScreen.Database.MediaDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeScreen.Database.MediaDb.Contexts;

public class MediaDbContext(DbContextOptions<MediaDbContext> options) : DbContext(options)
{
    public DbSet<MediaEntry> MediaEntries { get; set; }
}
