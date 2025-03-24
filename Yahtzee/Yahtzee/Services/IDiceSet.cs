using Yahtzee.Models;

namespace Yahtzee.Services;

public interface IDiceSet
{
    void Reset();
    int GetScore(CATEGORIES category);
    bool IsYahtzee();
    void RollDice();
}
