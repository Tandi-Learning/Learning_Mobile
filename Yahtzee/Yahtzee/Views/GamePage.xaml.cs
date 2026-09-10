using Yahtzee.Models;
using Yahtzee.ViewModels;

namespace Yahtzee.Views;

public partial class GamePage : ContentPage
{
    public GamePage(GamePageViewModel gamePageViewModel)
	{
		InitializeComponent();
		this.BindingContext = gamePageViewModel;
    }

	protected override void OnAppearing()
	{
		base.OnAppearing();
        //(this.BindingContext as GamePageViewModel)?.DiceSet.Reset();

		// if the last session is still in progress, load the state of the game
	}

    protected override void OnDisappearing()
	{
		base.OnDisappearing();

		// save the state of the game
	}
}