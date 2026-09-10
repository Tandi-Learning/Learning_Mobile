using Yahtzee.Models;

namespace Yahtzee.Interfaces;

public interface IDiceSet
{
    void Reset();
    int GetScore(CATEGORIES category);
    bool IsYahtzee();
    void RollDices();
    void ToggleDice(int position);
}
