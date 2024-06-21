using Yahtzee.Models;
namespace Yahtzee.Services;

public interface IScoreSheet
{
    void ResetAll();
    void SelectCategory(CATEGORIES category, int categoryScore);
    bool AssignCategoryScore(CATEGORIES category);
    CategoryScore CurrentSelectedCategory();
}
