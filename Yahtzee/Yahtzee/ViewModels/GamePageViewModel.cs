using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Yahtzee.Data;
using Yahtzee.Models;
using Yahtzee.Services;

namespace Yahtzee.ViewModels;

public partial class GamePageViewModel : BaseViewModel
{
    private readonly Services.INavigation navigation;
    private readonly HighScores highScores;
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
        Services.INavigation navigation,
        HighScores highScores
    )
    {
        DiceSet = diceSet;
        ScoreSheet = scoreSheet;
        this.navigation = navigation;
        this.highScores = highScores;
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
    public async Task SelectCategory(CATEGORIES selectedCategory)
    {
        var score = PreviousCategory != selectedCategory ? DiceSet.GetScore(selectedCategory) : 0;
        ScoreSheet.SelectCategory(selectedCategory, PreviousCategory, score);
        PreviousCategory = selectedCategory; // != PreviousCategory ? selectedCategory : CATEGORIES.NONE;

        var gameOver = ScoreSheet.IsGameOver();


        //if (selectedCategory == CATEGORIES.YAHTZEE)
        //    await highScores.ResetHighScoresAsync();
        //else
        //{
        //    await this.highScores.AddHighScoreAsync(new HighScore { Name = "Test", Score = 10, Date = DateTime.Now });
        //    IList<HighScore> scores = await highScores.GetHighScoresAsync();
        //}
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
        navigation.GotoMainPage();
    }

    private async void StartNewGame()
    {
        ResetDices();
        ScoreSheet.ResetAll();
        GameInProgress = true;
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
