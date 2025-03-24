using CommunityToolkit.Mvvm.ComponentModel;
using Yahtzee.Models;

namespace Yahtzee.Services;

public partial class ScoreSheet : ObservableObject, IScoreSheet
{
    private int yahtzeeCount = 0;

    [ObservableProperty]
    public string _yahtzeeCountLabel = "Yahtzee";

    [ObservableProperty]
    List<CategoryScore> _categoryScores = new(Enum.GetNames(typeof(CATEGORIES)).Length);

    public ScoreSheet()
    {
        foreach (CATEGORIES category in Enum.GetValues(typeof(CATEGORIES)))
        {
            CategoryScores.Add(new(category));
        }
    }

    public bool AssignCategoryScore(CATEGORIES category)
    {
        CategoryScores[(int)category].Assigned = true;
        CategoryScores[(int)category].Border = "Transparent";
        CategoryScores[(int)category].Selected = false;

        if (IsUpperCategory(category))
        {
            CategoryScores[(int)CATEGORIES.UPPER_SCORE].Score += CategoryScores[(int)category].Score;
            CategoryScores[(int)CATEGORIES.UPPER_BONUS].Score = CategoryScores[(int)CATEGORIES.UPPER_SCORE].Score >= 63 ? 35 : 0;
            CategoryScores[(int)CATEGORIES.UPPER_TOTAL].Score = CategoryScores[(int)CATEGORIES.UPPER_SCORE].Score + CategoryScores[(int)CATEGORIES.UPPER_BONUS].Score;
        }
        else if (IsLowerCategory(category))
        {
            if (category == CATEGORIES.YAHTZEE && CategoryScores[(int)CATEGORIES.YAHTZEE].Score == 50)
            {
                yahtzeeCount++;
                CategoryScores[(int)CATEGORIES.YAHTZEE_BONUS].Score += yahtzeeCount > 1 ? 100 : 0;
                YahtzeeCountLabel = $"Yahtzee ({yahtzeeCount})";
            }

            var categoryScore = yahtzeeCount > 1 ? 0 : CategoryScores[(int)category].Score;
            CategoryScores[(int)CATEGORIES.LOWER_SCORE].Score += categoryScore;
            CategoryScores[(int)CATEGORIES.LOWER_TOTAL].Score = CategoryScores[(int)CATEGORIES.LOWER_SCORE].Score + CategoryScores[(int)CATEGORIES.YAHTZEE_BONUS].Score;
        }

        CategoryScores[(int)CATEGORIES.GRAND_TOTAL].Score = CategoryScores[(int)CATEGORIES.UPPER_TOTAL].Score + CategoryScores[(int)CATEGORIES.LOWER_TOTAL].Score;

        var lastScore = CategoryScores.Count(c => !c.Assigned && Selectable(c.Category)) == 0;
        return lastScore;
    }

    public void ClearUnassignedCategory()
    {
        throw new NotImplementedException();
    }

    public void ResetAll()
    {
        foreach (CATEGORIES category in Enum.GetValues(typeof(CATEGORIES)))
        {
            CategoryScores[(int)category].Assigned = false;
            CategoryScores[(int)category].Border = "Transparent";
            CategoryScores[(int)category].Score = 0;
            CategoryScores[(int)category].Selected = false;
        }
    }

    public void SelectCategory(CATEGORIES selectedCategory, CATEGORIES previousCategory, int categoryScore)
    {
        CategoryScores[(int)selectedCategory].Border = CategoryScores[(int)selectedCategory].Border == "Red" ? "Transparent" : "Red";
        CategoryScores[(int)selectedCategory].Score = categoryScore;
        CategoryScores[(int)selectedCategory].Selected = true;
        if (selectedCategory != previousCategory)
        {
            CategoryScores[(int)previousCategory].Border = "Transparent";
            CategoryScores[(int)previousCategory].Score = 0;
            CategoryScores[(int)previousCategory].Selected = false;
        }
    }

    private bool IsUpperCategory(CATEGORIES category)
    {
        return category switch
        {
            CATEGORIES.ACES or
            CATEGORIES.TWOS or
            CATEGORIES.THREES or
            CATEGORIES.FOURS or
            CATEGORIES.FIVES or
            CATEGORIES.SIXES => true,
            _ => false
        };
    }

    private bool IsLowerCategory(CATEGORIES category)
    {
        return category switch
        {
            CATEGORIES.THREE_OF_A_KIND or
            CATEGORIES.FOUR_OF_A_KIND or
            CATEGORIES.FULLHOUSE or
            CATEGORIES.SMALL_STRAIGHT or
            CATEGORIES.LARGE_STRAIGHT or
            CATEGORIES.CHANCE or
            CATEGORIES.YAHTZEE => true,
            _ => false
        };
    }

    private bool Selectable(CATEGORIES category)
    {
        return category switch
        {
            CATEGORIES.ACES or
            CATEGORIES.TWOS or
            CATEGORIES.THREES or
            CATEGORIES.FOURS or
            CATEGORIES.FIVES or
            CATEGORIES.SIXES or
            CATEGORIES.THREE_OF_A_KIND or
            CATEGORIES.FOUR_OF_A_KIND or
            CATEGORIES.FULLHOUSE or
            CATEGORIES.SMALL_STRAIGHT or
            CATEGORIES.LARGE_STRAIGHT or
            CATEGORIES.CHANCE or
            CATEGORIES.YAHTZEE => true,
            _ => false
        };
    }

    public CategoryScore CurrentSelectedCategory()
    {
        return CategoryScores.Where(c => c.Selected).FirstOrDefault();
    }

    public bool IsGameOver()
    {
        return !CategoryScores.Any(c => c.Assigned == false);
    }
}
