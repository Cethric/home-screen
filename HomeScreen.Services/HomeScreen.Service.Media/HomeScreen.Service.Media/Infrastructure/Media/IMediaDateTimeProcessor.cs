namespace HomeScreen.Service.Media.Infrastructure.Media;

public interface IMediaDateTimeProcessor
{
    Task<(DateTimeOffset, TimeSpan)> MediaCaptureDate(
        FileInfo file,
        string hash,
        CancellationToken cancellationToken = default
    );
}
