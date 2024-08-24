using System.Globalization;
using HomeScreen.Database.MediaDb.Entities;
using HomeScreen.Service.Media.Infrastructure.Location;
using ImageMagick;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public class MediaProcessor(ILogger<MediaProcessor> logger, IMediaPaths mediaPaths, ILocationApi locationApi)
    : IMediaProcessor
{
    public async Task<Database.MediaDb.Entities.MediaEntry> ProcessMediaEntry(
        FileInfo file,
        string hash,
        CancellationToken cancellationToken
    )
{
    try
    {
        logger.LogInformation("Attempting to process media entry {FileName} - {Hash}", file.Name, hash);
        var stream = await File.ReadAllBytesAsync(file.FullName, cancellationToken);
        using var image = new MagickImage(stream);
        var profile = image.GetExifProfile();
        var entry = new Database.MediaDb.Entities.MediaEntry(file, hash) { Enabled = true };

        if (profile != null)
        {
            await ProcessLocation(profile, entry, cancellationToken);
            ProcessDateTime(profile, entry, file);
            entry.Notes = profile.GetValue(ExifTag.ImageDescription)?.Value ?? "";
        }


        var directory = mediaPaths.GetTransformDirectory(entry.OriginalHash);
        var originalInfo = new FileInfo(Path.Combine(directory.FullName, "original" + file.Extension));
        if (originalInfo.Exists)
        {
            return entry;
        }

        logger.LogInformation(
            "Writing Original File {OriginalFile} {CachedFile}",
            file.FullName,
            originalInfo.FullName
        );
        if (!originalInfo.Exists)
        {
            file.CopyTo(originalInfo.FullName);
        }

        return entry;
    }
    catch (IOException ex)
    {
        logger.LogError(ex, "Unable to process file - IOException {FileName}", file.FullName);
        throw;
    }
    catch (UnauthorizedAccessException ex)
    {
        logger.LogError(ex, "Unable to process file - AccessException {FileName}", file.FullName);
        throw;
    }
    catch (MagickMissingDelegateErrorException ex)
    {
        logger.LogError(ex, "Unable to process file - MissingDelegateException {FileName}", file.FullName);
        return new Database.MediaDb.Entities.MediaEntry(file, hash)
        {
            Notes = $"No magick handler provided {file.FullName}"
        };
    }
    catch (MagickCorruptImageErrorException ex)
    {
        logger.LogError(ex, "Unable to process file - CorruptImageException {FileName}", file.FullName);
        return new Database.MediaDb.Entities.MediaEntry(file, hash)
        {
            Notes = $"Corrupt image provided {file.FullName}"
        };
    }
}

private static void ProcessDateTime(IExifProfile profile, Database.MediaDb.Entities.MediaEntry entry, FileInfo info)
{
    if (DateTime.TryParseExact(
            profile.GetValue(ExifTag.DateTimeOriginal)?.Value,
            "yyyy:MM:dd HH:mm:ss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AllowWhiteSpaces,
            out var dateTimeOriginal
        ))
    {
        var offset = TimeSpan.TryParse(
            profile.GetValue(ExifTag.OffsetTimeOriginal)?.Value.Replace("+", ""),
            new CultureInfo("en-AU"),
            out var tryOffset
        )
            ? tryOffset
            : TimeSpan.Zero;
        entry.CapturedOffset = offset;
        entry.CapturedUtc = new DateTimeOffset(dateTimeOriginal, offset).ToOffset(TimeSpan.Zero);
    }
    else if (DateTime.TryParseExact(
                 profile.GetValue(ExifTag.DateTime)?.Value,
                 "yyyy:MM:dd HH:mm:ss",
                 CultureInfo.InvariantCulture,
                 DateTimeStyles.AllowWhiteSpaces,
                 out var dateTime
             ))
    {
        var offset = TimeSpan.TryParse(
            profile.GetValue(ExifTag.OffsetTime)?.Value.Replace("+", ""),
            new CultureInfo("en-AU"),
            out var tryOffset
        )
            ? tryOffset
            : TimeSpan.Zero;
        entry.CapturedOffset = offset;
        entry.CapturedUtc = new DateTimeOffset(dateTime, offset).ToOffset(TimeSpan.Zero);
    }
    else
    {
        if (info.LastWriteTimeUtc < info.CreationTimeUtc)
        {
            entry.CapturedUtc = info.LastWriteTimeUtc;
            entry.CapturedOffset = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time")
                                               .GetUtcOffset(info.LastWriteTime);
        }
        else
        {
            entry.CapturedUtc = info.CreationTimeUtc;
            entry.CapturedOffset = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time")
                                               .GetUtcOffset(info.CreationTime);
        }
    }
}

private async Task ProcessLocation(
    IExifProfile profile,
    Database.MediaDb.Entities.MediaEntry entry,
    CancellationToken cancellationToken
)
{
    if (profile.GetValue(ExifTag.GPSLongitude) == null)
    {
        return;
    }

    var lonRef = profile.GetValue(ExifTag.GPSLongitudeRef)?.Value;
    entry.LongitudeDirection = GpsLongitudeRefToLongitudeDirection(lonRef);
    var latRef = profile.GetValue(ExifTag.GPSLatitudeRef)?.Value;
    entry.LatitudeDirection = GpsLatitudeRefToLatitudeDirection(latRef);
    if (profile.GetValue(ExifTag.GPSLongitude)?.Value is { } longitude)
    {
        var longitudeDegrees = longitude[0].ToDouble();
        var longitudeMinutes = longitude[1].ToDouble();
        var longitudeSeconds = longitude[2].ToDouble();

        entry.Longitude = longitudeDegrees + longitudeMinutes / 60 + longitudeSeconds / 3600;
        entry.Longitude = entry.LongitudeDirection == LongitudeDirection.East ? entry.Longitude : -entry.Longitude;
    }

    if (profile.GetValue(ExifTag.GPSLatitude)?.Value is { } latitude)
    {
        var latitudeDegrees = latitude[0].ToDouble();
        var latitudeMinutes = latitude[1].ToDouble();
        var latitudeSeconds = latitude[2].ToDouble();

        entry.Latitude = latitudeDegrees + latitudeMinutes / 60 + latitudeSeconds / 3600;
        entry.Latitude = entry.LatitudeDirection == LatitudeDirection.North ? entry.Latitude : -entry.Latitude;
    }

    var altitude = profile.GetValue(ExifTag.GPSAltitude)?.Value.ToDouble() ?? 0d;
    var aboveSea = profile.GetValue(ExifTag.GPSAltitudeRef)?.Value == 0;

    entry.LocationLabel = await locationApi.SearchForLocation(
        entry.Longitude,
        entry.Latitude,
        aboveSea ? altitude : -altitude,
        cancellationToken
    );
}

private static LongitudeDirection GpsLongitudeRefToLongitudeDirection(string? lonRef)
{
    return lonRef switch
    {
        "W" => LongitudeDirection.West,
        "E" => LongitudeDirection.East,
        _ => throw new ArgumentOutOfRangeException(
            nameof(lonRef),
            lonRef,
            "Invalid longitude reference value provided"
        )
    };
}

private static LatitudeDirection GpsLatitudeRefToLatitudeDirection(string? latRef)
{
    return latRef switch
    {
        "N" => LatitudeDirection.North,
        "S" => LatitudeDirection.South,
        _ => throw new ArgumentOutOfRangeException(
            nameof(latRef),
            latRef,
            "Invalid latitude reference value provided"
        )
    };
}
}
