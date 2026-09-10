using CommunityToolkit.Maui.Layouts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahtzee.Models;
using Yahtzee.Services;
using Yahtzee.Views;

namespace Yahtzee.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    private readonly Services.INavigation navigation;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ResumeGameCommand))]
    private bool _gameInProgress;

    public MainPageViewModel(Services.INavigation navigation)
    {
        this.navigation = navigation;
    }

    [RelayCommand]
    private async Task NewGame()
    { 
        GameInProgress = true;
        await navigation.GotoGamePage();
        Preferences.Set("Fullname", "Scarly");
    }

    [RelayCommand(CanExecute = nameof(CanResumeGame))]
    private async Task ResumeGame()
    {
        await navigation.GotoGamePage();
    }

    public bool CanResumeGame()
    {
        return GameInProgress;
    }

    [RelayCommand]
    private async Task ViewScores()
    {
        await navigation.GotoScoresPage();
    }

    [RelayCommand]
    private async Task ExitGame()
    {
        await navigation.GotoScoresPage();
    }
}