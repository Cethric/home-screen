using MetadataExtractor.Formats.Exif;
using Directory = MetadataExtractor.Directory;

namespace HomeScreen.Service.Media.Worker.Infrastructure;

public interface IMediaMetadataReader
{
    IReadOnlyList<Directory> LoadMetadata(FileInfo file);

    Task<ExifSubIfdDirectory?> LoadExif(FileInfo file, CancellationToken cancellationToken);

    Task<GpsDirectory?> LoadGps(FileInfo file, CancellationToken cancellationToken);
}
