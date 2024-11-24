using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Yahtzee.Models;

namespace Yahtzee.Services;

public partial class DiceSet : ObservableObject, IDiceSet
{
    [ObservableProperty]
    public List<Dice> _dices = new(5);

    public DiceSet()
    {
        foreach(DICE_POSITION position in Enum.GetValues(typeof(DICE_POSITION)))
        {
            Dices.Add(new(position));
        }
    }

    [RelayCommand]
    public void HoldDice(DICE_POSITION position)
    {
        Dices[(int)position].Hold = !Dices[(int)position].Hold;
    }

    public int GetScore(CATEGORIES category)
    {
        return category switch
        {
            CATEGORIES.ACES or
            CATEGORIES.TWOS or
            CATEGORIES.THREES or
            CATEGORIES.FOURS or
            CATEGORIES.FIVES or
            CATEGORIES.SIXES
            => UpperCategoryScore(category),
            CATEGORIES.THREE_OF_A_KIND => SameOfAKindScore(3),
            CATEGORIES.FOUR_OF_A_KIND => SameOfAKindScore(4),
            CATEGORIES.SMALL_STRAIGHT or 
            CATEGORIES.LARGE_STRAIGHT => StraightScore(category),
            CATEGORIES.FULLHOUSE => FullhouseScore(),
            CATEGORIES.CHANCE => ChanceScore(),
            CATEGORIES.YAHTZEE => YahtzeeScore(),
            _ => 0
        };
    }

    private int UpperCategoryScore(CATEGORIES category)
    {
        // Upper Bonus is 35 is Upper Total >= 63
        int categoryValue = (int)category + 1;
        return Dices.Where(d => d.Value == categoryValue).Count() * categoryValue;
    }

    private int SameOfAKindScore(int sameCount)
    {
        var grouped = Dices.GroupBy(
            d => d.Value,
            (key, dice) => new
            {
                Key = key,
                GroupCount = dice.Count()
            });

        if (grouped.Max(g => g.GroupCount) >= sameCount)
            return Dices.Sum(d => d.Value);
        else
            return 0;
    }

    private int StraightScore(CATEGORIES category)
    {
        int compareSeq = category == CATEGORIES.SMALL_STRAIGHT ? 4 : 5;
        int score = category == CATEGORIES.SMALL_STRAIGHT ? 30 : 40;
        int seq = 0;
        int currentValue = 0;
        List<int> values = Dices.Select(d => d.Value).Order().Distinct().ToList();

        bool isStraight = false;
        foreach (var value in values)
        {
            seq = (currentValue == 0 || currentValue == value - 1) ? seq + 1 : 0;
            isStraight = (category == CATEGORIES.SMALL_STRAIGHT && seq == 4) || 
                (category == CATEGORIES.LARGE_STRAIGHT && seq == 5);
            if (isStraight) break;
            currentValue = value;
        }

        return isStraight ? score : 0;
    }

    private int FullhouseScore() 
    {
        var grouped = GroupDiceByScore();

        if (grouped.Count() == 2  && 
            grouped.Any(g => g.GroupCount == 2) &&
            grouped.Any(g => g.GroupCount == 3))
        {
            return 25;
        }
        else 
        { 
            return 0; 
        }
    }

    private int ChanceScore() 
    {
        return Dices.Sum(d => d.Value);
    }

    private int YahtzeeScore() 
    {
        return IsYahtzee() ? 50 : 0;
    }

    public bool IsYahtzee()
    {
        var grouped = GroupDiceByScore();
        return grouped.Any(g => g.GroupCount == 5);
    }

    private IEnumerable<ScoreGroup> GroupDiceByScore()
    {
        var grouped = Dices.GroupBy(
            d => d.Value,
            (key, dice) => new ScoreGroup
            {
                Key = key,
                GroupCount = dice.Count()
            });

        return grouped;
    }

    public void Reset()
    {
        foreach (DICE_POSITION position in Enum.GetValues(typeof(DICE_POSITION)))
        {
            Dice dice = Dices.First(d => d.Position == position);
            dice.Hold = false;
            dice.Value = 0;
            dice.Image = "dice_blank.png";
        }
    }

    public void RollDice()
    {
        Random random = new Random();        
        foreach (DICE_POSITION position in Enum.GetValues(typeof(DICE_POSITION)))
        {
            Dice dice = Dices.First(d => d.Position == position);
            var randomValue = random.Next(1, 7);
            if (!dice.Hold)
            {
                dice.Value = randomValue;
                dice.Image = $"dice_{randomValue}.png";
                dice.Hold = false;
            }
        }
    }
}
