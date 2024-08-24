using HomeScreen.Service.Media.Entities;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public interface IMediaTransformer
{
    Task<FileInfo> GetTransformedMedia(
        Database.MediaDb.Entities.MediaEntry mediaEntry,
        MediaTransformOptions options,
        CancellationToken cancellationToken
    );
}
