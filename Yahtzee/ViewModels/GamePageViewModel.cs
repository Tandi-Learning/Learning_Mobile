using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Yahtzee.Models;
using Yahtzee.Services;

namespace Yahtzee.ViewModels;

public partial class GamePageViewModel : BaseViewModel
{
    private readonly INavigationService navigationService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RollDicesCommand))]
    private CATEGORIES _previousCategory = CATEGORIES.NONE;

    [ObservableProperty]
    public DiceSet _diceSet;

    [ObservableProperty]
    public ScoreSheet _scoreSheet;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RollDicesCommand))]
    [NotifyCanExecuteChangedFor(nameof(SelectCategoryCommand))]
    public int _rollCount = 0;

    [ObservableProperty]
    public ObservableCollection<string> _rollCountImages = new() { CONSTANTS.GREEN_LED, CONSTANTS.GREEN_LED, CONSTANTS.GREEN_LED };

    public GamePageViewModel(
        DiceSet diceSet,
        ScoreSheet scoreSheet,
        INavigationService navigationService
    )
    {
        DiceSet = diceSet;
        ScoreSheet = scoreSheet;
        this.navigationService = navigationService;
    }

    [RelayCommand(CanExecute = nameof(CanRollDice))]
    public void RollDices()
    {
        bool lastScore = false;
        if (PreviousCategory != CATEGORIES.NONE)
        {
            lastScore = ScoreSheet.AssignCategoryScore(PreviousCategory);
            ResetDices();            
        }

        if (!lastScore)
        {
            RollCountImages[RollCount] = CONSTANTS.RED_LED;
            RollCount++;
            DiceSet.RollDice();
        }
    }

    private bool CanRollDice()
    {
        return RollCount < 3 || PreviousCategory != CATEGORIES.NONE;
    }

    [RelayCommand(CanExecute = nameof(CanSelectCategory))]
    public void SelectCategory(CATEGORIES selectedCategory)
    {
        var score = PreviousCategory != selectedCategory ? DiceSet.GetScore(selectedCategory) : 0;
        ScoreSheet.SelectCategory(selectedCategory, PreviousCategory, score);
        PreviousCategory = selectedCategory; // != PreviousCategory ? selectedCategory : CATEGORIES.NONE;
    }

    private bool CanSelectCategory(CATEGORIES category) 
    {
        return RollCount > 0 && (
            !ScoreSheet.CategoryScores[(int)category].Assigned || 
            (category == CATEGORIES.YAHTZEE && ScoreSheet.CategoryScores[(int)category].Score == 50)
        );
    }

    [RelayCommand]
    public void BackToMain()
    {
        navigationService.GotoMainPage();
    }

    private void StartNewGame()
    {
        ResetDices();
        ScoreSheet.ResetAll();
    }

    [RelayCommand]
    void Appearing()
    {
        try
        {
            StartNewGame();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
    }

    [RelayCommand]
    void Disappearing()
    {
        try
        {
            // DoSomething
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
    }

    private void ResetDices()
    {
        RollCount = 0;
        for (int i = 0; i < 3; i++)
            RollCountImages[i] = CONSTANTS.GREEN_LED;
        PreviousCategory = CATEGORIES.NONE;
        DiceSet.Reset();
    }
}
