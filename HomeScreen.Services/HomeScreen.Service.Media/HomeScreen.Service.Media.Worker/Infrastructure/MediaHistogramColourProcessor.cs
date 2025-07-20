using System.Diagnostics;
using ImageMagick;
using ImageMagick.Colors;

namespace HomeScreen.Service.Media.Worker.Infrastructure;

public class MediaHistogramColourProcessor : IMediaColourProcessor
{
    private static ActivitySource ActivitySource => new(nameof(MediaHistogramColourProcessor));
    
    public (byte r, byte g, byte b, byte a) GetBaseImageColour(MagickImage image)
    {
        using var activity = ActivitySource.StartActivity();
        image.FilterType = FilterType.Point;
        image.Alpha(AlphaOption.Remove);
        image.Thumbnail(32, 32);
        image.MedianFilter(4);
        var histogram = image
            .Histogram()
            .OrderByDescending(entry => entry.Value)
            .FirstOrDefault(entry => !entry.Key.IsCmyk);
        var colour = ColorRGB.FromMagickColor(histogram.Key)?.ToMagickColor().ToByteArray();
        return colour is not null
            ? (colour[0], colour[1], colour[2], colour[3])
            : ((byte r, byte g, byte b, byte a))(0, 0, 0, 0);
    }
}
