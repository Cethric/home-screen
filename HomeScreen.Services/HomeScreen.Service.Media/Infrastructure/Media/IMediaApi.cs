using HomeScreen.Service.Media.Entities;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public interface IMediaApi
{
    Task<IList<MediaEntry>> GetRandomMedia(uint count, CancellationToken cancellationToken);

    Task<MediaEntry> ToggleMedia(Guid mediaId, bool state, CancellationToken cancellationToken);

    Task<FileInfo?> GetTransformedMedia(
        Guid mediaId,
        MediaTransformOptions options,
        CancellationToken cancellationToken
    );
}
