namespace HomeScreen.Service.Media.Worker.Infrastructure;

public interface IMediaApi
{
    Task ProcessMedia(CancellationToken cancellationToken);
}
