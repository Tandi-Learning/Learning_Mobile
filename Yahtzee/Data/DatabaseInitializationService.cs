using Microsoft.Extensions.Logging;
using SQLite;
using Yahtzee.Models;

namespace Yahtzee.Data;

/// <summary>
/// Creates the local SQLite database schema from the application model classes.
/// </summary>
public sealed class DatabaseInitializationService
{
    private readonly ILogger<DatabaseInitializationService> _logger;

    public DatabaseInitializationService(ILogger<DatabaseInitializationService> logger)
    {
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            SQLitePCL.Batteries_V2.Init();
        }
        catch (Exception exception)
        {
            _logger.LogDebug(exception, "SQLite native provider was already initialized.");
        }

        try
        {
            SQLiteAsyncConnection connection = SqliteService.GetConnection();

            await connection.CreateTableAsync<Category>(CreateFlags.None).ConfigureAwait(false);
            await connection.CreateTableAsync<Project>(CreateFlags.None).ConfigureAwait(false);
            await connection.CreateTableAsync<ProjectTask>(CreateFlags.None).ConfigureAwait(false);
            await connection.CreateTableAsync<Tag>(CreateFlags.None).ConfigureAwait(false);
            await connection.CreateTableAsync<ProjectsTags>(CreateFlags.None).ConfigureAwait(false);
            await connection.CreateTableAsync<GameState>(CreateFlags.None).ConfigureAwait(false);

            _logger.LogInformation("SQLite database initialized at {DatabasePath}.", Constants.DatabasePath);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error initializing the SQLite database.");
            throw;
        }
    }
}
