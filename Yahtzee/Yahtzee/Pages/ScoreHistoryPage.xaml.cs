using Yahtzee.PageModels;

namespace Yahtzee.Pages;

public partial class ScoreHistoryPage : ContentPage
{
    public ScoreHistoryPage(ScoreHistoryPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
