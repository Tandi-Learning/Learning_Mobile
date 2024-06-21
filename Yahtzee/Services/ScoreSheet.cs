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

    public void SelectCategory(CATEGORIES selectedCategory, int categoryScore)
    {       
        foreach (var category in CategoryScores)
        {
            if ((!category.Assigned || selectedCategory == CATEGORIES.YAHTZEE) && Selectable(category.Category))
            {
                int unselectedScore = selectedCategory == CATEGORIES.YAHTZEE && category.Assigned ? 50 : 0;
                category.Selected = category.Category != selectedCategory ? false : !category.Selected;
                category.Border = category.Selected ? "Red" : "Transparent";
                category.Score = category.Selected ? categoryScore : unselectedScore;
                ;
            }
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
}