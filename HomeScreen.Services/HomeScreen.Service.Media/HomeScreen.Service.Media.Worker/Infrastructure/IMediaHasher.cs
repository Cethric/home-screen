namespace HomeScreen.Service.Media.Worker.Infrastructure;

public interface IMediaHasher
{
    Task<string> HashMedia(FileInfo fileInfo, CancellationToken cancellationToken = default);
}
