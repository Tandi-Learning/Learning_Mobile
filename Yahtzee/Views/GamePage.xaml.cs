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
    }

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
	}
}