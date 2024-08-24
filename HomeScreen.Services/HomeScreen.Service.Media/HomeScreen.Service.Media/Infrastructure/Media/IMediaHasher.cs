namespace HomeScreen.Service.Media.Infrastructure.Media;

public interface IMediaHasher
{
    Task<string> HashMedia(FileInfo fileInfo, CancellationToken cancellationToken = default);
}
