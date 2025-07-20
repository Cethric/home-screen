namespace HomeScreen.Service.Media.Worker.Infrastructure;

public interface IMediaDateTimeProcessor
{
    Task<(DateTimeOffset, TimeSpan)> MediaCaptureDate(
        FileInfo file,
        CancellationToken cancellationToken = default
    );
}
