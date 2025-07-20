namespace HomeScreen.Service.Media.Common.Infrastructure.Media;

public interface IMediaPaths
{
    DirectoryInfo GetTransformDirectory(string fileHash);

    IAsyncEnumerable<string> GetRawFiles();
}
