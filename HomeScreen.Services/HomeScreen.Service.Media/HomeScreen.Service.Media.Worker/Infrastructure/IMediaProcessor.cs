namespace HomeScreen.Service.Media.Worker.Infrastructure;

public interface IMediaProcessor
{
    Task<Database.MediaDb.Entities.MediaEntry> ProcessMediaEntry(
        FileInfo file,
        string hash,
        CancellationToken cancellationToken = default
    );
}
