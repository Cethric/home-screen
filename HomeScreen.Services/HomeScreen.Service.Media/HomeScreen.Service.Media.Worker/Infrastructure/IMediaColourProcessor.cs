using ImageMagick;

namespace HomeScreen.Service.Media.Worker.Infrastructure;

public interface IMediaColourProcessor
{
    (byte r, byte g, byte b, byte a) GetBaseImageColour(MagickImage image);
}
