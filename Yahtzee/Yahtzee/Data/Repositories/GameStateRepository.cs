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

    /// <summary>
    /// Writes the whole row. There is only ever one, so the id is stamped here rather than trusted from
    /// the caller.
    /// </summary>
    public Task SaveAsync(GameState gameState)
    {
        gameState.ID = CurrentGameId;
        return Connection.InsertOrReplaceAsync(gameState);
    }

    /// <summary>
    /// Discards any saved game. A new game starts from no row at all rather than from a half-filled one:
    /// the page model builds a fresh board when it finds nothing, and the first action writes a real row.
    /// The dashboard stops offering "Resume" until that new board has something to restore.
    /// </summary>
    public Task ClearAsync() => Connection.DeleteAsync<GameState>(CurrentGameId);
}
