using HomeScreen.Service.Media.Worker.Infrastructure;

namespace HomeScreen.Service.Media.Worker;

public class Worker(ILogger<Worker> logger, IServiceProvider provider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await ProcessMedia(stoppingToken);

            using var timer = new PeriodicTimer(TimeSpan.FromDays(7));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ProcessMedia(stoppingToken);
            }
        }
        catch (OperationCanceledException ex)
        {
            logger.LogWarning(ex, "Service stopping...");
        }
    }

    private async Task ProcessMedia(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            logger.LogInformation("Processing media");
            await using var scope = provider.CreateAsyncScope();
            var api = scope.ServiceProvider.GetRequiredService<IMediaApi>();
            await api.ProcessMedia(cancellationToken);
            logger.LogInformation("Media Processed");
        }
        catch (OperationCanceledException ex)
        {
            logger.LogInformation(ex, "Service Cancelled");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process task");
        }
    }
}
