using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahtzee.Services;
using Yahtzee.Views;

namespace Yahtzee.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;

    public MainPageViewModel(INavigationService navigationService)
    {
        this._navigationService = navigationService;
    }

    [RelayCommand]
    private async Task NewGame()
    {
        await _navigationService.GotoGamePage();
    }

    [RelayCommand]
    private async Task ResumeGame()
    {
        await _navigationService.GotoGamePage();
    }

    [RelayCommand]
    private async Task ViewScores()
    {
        await _navigationService.GotoScoresPage();
    }

    [RelayCommand]
    private async Task ExitGame()
    {
        await _navigationService.GotoScoresPage();
    }
}
