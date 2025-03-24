using Yahtzee.ViewModels;

namespace Yahtzee.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel mainPageViewModel)
    {
        InitializeComponent();
        this.BindingContext = mainPageViewModel;
    }
}