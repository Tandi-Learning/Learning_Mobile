using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahtzee.Data;

namespace Yahtzee.Services;

public class HighScores : IHighScores
{
    private readonly YahtzeeDatabase database;

    public HighScores(YahtzeeDatabase database)
    {
        this.database = database;
    }

    public async Task<List<HighScore>> GetHighScoresAsync()
    {
        return await database.GetHighScoresAsync();
    }

    public async Task<int> AddHighScoreAsync(HighScore highScore)
    {
        return await database.AddHighScoreAsync(highScore);
    }

    public async Task ResetHighScoresAsync()
    {
        await database.ResetHighScoresAsync();
    }
}
