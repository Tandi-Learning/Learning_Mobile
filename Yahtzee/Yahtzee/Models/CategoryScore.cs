using CommunityToolkit.Mvvm.ComponentModel;

namespace Yahtzee.Models;

public partial class CategoryScore : ObservableObject
{ 
    [ObservableProperty]
    public CATEGORIES _category;
    [ObservableProperty]
    public int _score = 0;
    [ObservableProperty]
    public bool _assigned = false;
    [ObservableProperty]
    public bool _selected = false;
    [ObservableProperty]
    public bool _show = true;
    [ObservableProperty]
    public string _border = "Transparent";

    public CategoryScore(CATEGORIES Category)
    {
        this.Category = Category;
        var Selectable = Category switch
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
            CATEGORIES.YAHTZEE => false,
            _ => true
        };
    }
}
