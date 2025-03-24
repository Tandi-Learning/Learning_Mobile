using Yahtzee.Models;
namespace Yahtzee.Services;

public interface IScoreSheet
{
    void ResetAll();
    void SelectCategory(CATEGORIES newCategory, CATEGORIES selectedCategory, int categoryScore);
    bool AssignCategoryScore(CATEGORIES category);
    CategoryScore CurrentSelectedCategory();
    bool IsGameOver();
}
