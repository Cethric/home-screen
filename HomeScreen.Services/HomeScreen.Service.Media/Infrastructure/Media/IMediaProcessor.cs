namespace HomeScreen.Service.Media.Infrastructure.Media;

public interface IMediaProcessor
{
    Task<Database.MediaDb.Entities.MediaEntry> ProcessMediaEntry(
        FileInfo file,
        string hash,
        CancellationToken cancellationToken
    );
}
