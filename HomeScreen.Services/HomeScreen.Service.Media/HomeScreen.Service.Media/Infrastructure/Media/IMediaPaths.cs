using HomeScreen.Service.Media.Entities;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public interface IMediaPaths
{
    DirectoryInfo GetTransformDirectory(string fileHash);

    FileInfo GetCachePath(MediaTransformOptions mediaTransformOptions, string fileHash);

    Task<IEnumerable<FileInfo>> GetRawFiles(CancellationToken cancellationToken = default);

    Task<ulong> TotalMedia(CancellationToken cancellationToken = default);
}
