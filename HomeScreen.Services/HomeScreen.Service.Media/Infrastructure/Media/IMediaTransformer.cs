using HomeScreen.Service.Media.Entities;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public interface IMediaTransformer
{
    Task<TransformState> TransformedMedia(
        Database.MediaDb.Entities.MediaEntry mediaEntry,
        MediaTransformOptions options,
        CancellationToken cancellationToken
    );

    FileInfo? GetTransformedMedia(
        Database.MediaDb.Entities.MediaEntry mediaEntry,
        MediaTransformOptions options
    );
}
