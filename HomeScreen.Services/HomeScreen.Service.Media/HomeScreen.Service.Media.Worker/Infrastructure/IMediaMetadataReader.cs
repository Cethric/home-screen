using MetadataExtractor.Formats.Exif;
using Directory = MetadataExtractor.Directory;

namespace HomeScreen.Service.Media.Worker.Infrastructure;

public interface IMediaMetadataReader
{
    IReadOnlyList<Directory> LoadMetadata(FileInfo file);

    Task<Directory?> LoadExif(FileInfo file, CancellationToken cancellationToken);

    Task<GpsDirectory?> LoadGPS(FileInfo file, CancellationToken cancellationToken);
}
