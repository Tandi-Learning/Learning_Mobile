using Yahtzee.Data;

namespace Yahtzee.Services;

public interface IHighScores
{
    public Task<List<HighScore>> GetHighScoresAsync();
    public Task<int> AddHighScoreAsync(HighScore highScore);
    public Task ResetHighScoresAsync();
}