using HomeScreen.Database.MediaDb.Entities;
using ImageMagick;

namespace HomeScreen.Service.Media.Worker.Infrastructure;

public interface IMediaLocationProcessor
{
    Task<(double, double, string?)> ProcessLocation(FileInfo file, CancellationToken cancellationToken);
}
