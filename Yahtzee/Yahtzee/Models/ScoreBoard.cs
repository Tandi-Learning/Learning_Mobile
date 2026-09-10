using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Yahtzee.Models;

public partial class ScoreBoard : ObservableObject
{
    [ObservableProperty]
    public ObservableCollection<CategoryScore> _scores =
    [
        new CategoryScore(CATEGORIES.ACES),
        new CategoryScore(CATEGORIES.TWOS),
        new CategoryScore(CATEGORIES.THREES),
        new CategoryScore(CATEGORIES.FOURS),
        new CategoryScore(CATEGORIES.FIVES),
        new CategoryScore(CATEGORIES.SIXES),
        new CategoryScore(CATEGORIES.THREE_OF_A_KIND),
        new CategoryScore(CATEGORIES.FOUR_OF_A_KIND),
        new CategoryScore(CATEGORIES.FULLHOUSE),
        new CategoryScore(CATEGORIES.SMALL_STRAIGHT),
        new CategoryScore(CATEGORIES.LARGE_STRAIGHT),
        new CategoryScore(CATEGORIES.YAHTZEE),
        new CategoryScore(CATEGORIES.CHANCE)
    ];
}
