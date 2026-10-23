using SQLite;
using Yahtzee.Data;
using Yahtzee.Models;

namespace Yahtzee.Data.Repositories;

/// <summary>
/// Stores the state of the single resumable game.
/// </summary>
public sealed class GameStateRepository
{
    private const int CurrentGameId = 1;

    private SQLiteAsyncConnection Connection => SqliteService.GetConnection();

    public Task<GameState?> GetAsync()
        => Connection.Table<GameState>()
            .Where(gameState => gameState.ID == CurrentGameId)
            .FirstOrDefaultAsync();

    public async Task<bool> IsGameInProgressAsync()
        => (await GetAsync())?.IsInProgress == true;

    public Task SetInProgressAsync(bool isInProgress)
        => Connection.InsertOrReplaceAsync(new GameState
        {
            ID = CurrentGameId,
            IsInProgress = isInProgress
        });
}
