using CommunityToolkit.Maui.Layouts;
using Yahtzee.ViewModels;

namespace Yahtzee.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel mainPageViewModel)
    {
        InitializeComponent();
        this.BindingContext = mainPageViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();        
    }
}