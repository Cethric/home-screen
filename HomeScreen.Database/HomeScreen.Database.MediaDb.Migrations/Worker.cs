using System.Diagnostics;
using HomeScreen.Database.MediaDb.Contexts;
using HomeScreen.Database.MediaDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using OpenTelemetry.Trace;

namespace HomeScreen.Database.MediaDb.Migrations;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger<Worker> logger
) : BackgroundService
{
    private const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);
        logger.LogInformation("Migrating database");

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MediaDbContext>();

            await EnsureDatabaseAsync(dbContext, stoppingToken);
            await RunMigrationAsync(dbContext, stoppingToken);
            await SeedDataAsync(dbContext, stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database");
            activity?.RecordException(ex);
            throw;
        }

        logger.LogInformation("Migrated database");
        hostApplicationLifetime.StopApplication();
    }

    private static async Task EnsureDatabaseAsync(MediaDbContext dbContext, CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(
            async () =>
            {
                // Create the database if it does not exist.
                // Do this first so there is then a database to start a transaction against.
                if (!await dbCreator.ExistsAsync(cancellationToken))
                {
                    await dbCreator.CreateAsync(cancellationToken);
                }
            }
        );
    }

    private static async Task RunMigrationAsync(MediaDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(
            async () =>
            {
                // Run migration in a transaction to avoid partial migration if it fails.
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
                await dbContext.Database.MigrateAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
        );
    }

    private static async Task SeedDataAsync(MediaDbContext dbContext, CancellationToken cancellationToken)
    {
        MediaEntry firstEntry = new() { MediaId = Guid.NewGuid() };

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(
            async () =>
            {
                // Seed the database
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
                await dbContext.MediaEntries.AddAsync(firstEntry, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
        );
    }
}
