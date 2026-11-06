using SQLite;
using Yahtzee.Data;
using Yahtzee.Models;

namespace Yahtzee.Data.Repositories;

/// <summary>
/// Stores the best <see cref="LeaderboardSize"/> finished games. The table is kept to exactly that many: a
/// score that would not make the cut is not written at all, so only leaderboard scores are persisted.
/// </summary>
public sealed class ResultRepository
{
    /// <summary>How many scores the leaderboard keeps, and therefore how many it shows.</summary>
    public const int LeaderboardSize = 10;

    private SQLiteAsyncConnection Connection => SqliteService.GetConnection();

    /// <summary>The best <paramref name="max"/> scores, highest first; ties broken by older date.</summary>
    public async Task<IReadOnlyList<GameResult>> ListTopAsync(int max)
    {
        return await Connection.QueryAsync<GameResult>(
            "SELECT * FROM GameResult ORDER BY Score DESC, FinishedAtTicks ASC LIMIT @p0",
            new object[] { max });
    }

    /// <summary>
    /// Inserts a leaderboard score and trims the table back to the best <paramref name="max"/> rows, so a
    /// table left over from a larger limit cannot keep rows below the cut.
    /// </summary>
    public async Task<int> InsertTopAsync(GameResult result, int max)
    {
        int id = await Connection.InsertAsync(result);

        await Connection.ExecuteAsync(
            "DELETE FROM GameResult WHERE ID NOT IN "
            + "(SELECT ID FROM GameResult ORDER BY Score DESC, FinishedAtTicks ASC LIMIT @p0)",
            new object[] { max });

        return id;
    }

    /// <summary>True when <paramref name="score"/> would be among the best <paramref name="max"/> stored.</summary>
    public async Task<bool> IsInTopNAsync(int score, int max)
    {
        IReadOnlyList<GameResult> top = await ListTopAsync(max);
        if (top.Count < max)
        {
            return true;
        }

        int lowest = top.Min(r => r.Score);
        return score >= lowest;
    }

    /// <summary>Updates only the name on an existing row.</summary>
    public Task UpdateNameAsync(int id, string name)
        => Connection.ExecuteAsync(
            "UPDATE GameResult SET Name = @name WHERE ID = @id",
            new { name, id });
}
