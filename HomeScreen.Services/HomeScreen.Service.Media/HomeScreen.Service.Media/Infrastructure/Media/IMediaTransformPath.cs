using HomeScreen.Service.Media.Entities;

namespace HomeScreen.Service.Media.Infrastructure.Media;

public interface IMediaTransformPath
{
    FileInfo GetCachePath(MediaTransformOptions mediaTransformOptions, string fileHash);
}