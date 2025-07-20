using HomeScreen.Database.MediaDb.Entities;
using ImageMagick;

namespace HomeScreen.Service.Media.Worker.Infrastructure;

public interface IMediaLocationProcessor
{
    Task<(LongitudeDirection, LatitudeDirection, double, double, string)> ProcessLocation(
        FileInfo file,
        CancellationToken cancellationToken
    );
}
