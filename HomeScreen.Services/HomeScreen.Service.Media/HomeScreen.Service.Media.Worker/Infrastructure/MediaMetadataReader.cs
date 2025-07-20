using System.Diagnostics;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Directory = MetadataExtractor.Directory;

namespace HomeScreen.Service.Media.Worker.Infrastructure;

public class MediaMetadataReader(ILogger<MediaMetadataReader> logger) : IMediaMetadataReader
{
    private static ActivitySource ActivitySource => new(nameof(MediaDateTimeProcessor));
    
    public IReadOnlyList<Directory> LoadMetadata(FileInfo file)
    {
        using var activity = ActivitySource.StartActivity();
        logger.LogDebug("Determining media metadata {FileName}", file.FullName);
        try
        {
            var metadata = ImageMetadataReader.ReadMetadata(file.OpenRead(), file.Name);
            return metadata;
        }
        catch (IOException ex)
        {
            logger.LogError(ex, "Unable to process file - IOException {FileName}", file.FullName);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogError(ex, "Unable to process file - AccessException {FileName}", file.FullName);
        }

        return [];
    }

    public async Task<Directory?> LoadExif(FileInfo file, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity();
        var metadata = LoadMetadata(file);
        var exif = await metadata
            .ToAsyncEnumerable()
            .Where(data => string.Equals(data.Name, "Exif SubIfd", StringComparison.InvariantCultureIgnoreCase))
            .FirstOrDefaultAsync(cancellationToken);
        return exif;
    }

    public async Task<GpsDirectory?> LoadGPS(FileInfo file, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity();
        var metadata = LoadMetadata(file);
        var gps = await metadata
            .ToAsyncEnumerable()
            .Where(data => string.Equals(data.Name, "GPS", StringComparison.InvariantCultureIgnoreCase))
            .Select(data=>data as GpsDirectory)
            .FirstOrDefaultAsync(cancellationToken);
        return gps;
    }
}
