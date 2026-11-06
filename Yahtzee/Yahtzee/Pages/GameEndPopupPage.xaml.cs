using Microsoft.Maui.Controls;

namespace Yahtzee.Pages;

public partial class GameEndPopupPage : ContentPage
{
    public GameEndPopupPage()
    {
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        // The end-game dialog is a modal: the player must choose Play Again or Quit.
        // Prevent the Shell back button from popping the dialog without a choice.
        return true;
    }
}
