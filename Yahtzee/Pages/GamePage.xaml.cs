using Yahtzee.PageModels;

namespace Yahtzee.Pages;

public partial class GamePage : ContentPage
{
    public GamePage(GamePageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
