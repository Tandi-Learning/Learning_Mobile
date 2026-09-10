using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Yahtzee.Models;
using Yahtzee.Services;

namespace Yahtzee.Messaging;

internal partial class ScoreBoardPanelViewModel : ObservableObject
{    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SelectCategoryCommand))]
    private bool _diceRollNotified = false;

    [ObservableProperty]
    public ScoreBoard _scoreBoard = new();

    // can't figure out how to work with this custom observable dictionary
    //[ObservableProperty]
    //public ObservableDictionary<CATEGORIES, CategoryScore> _scoreBoardDict = new()
    //{
    //    { CATEGORIES.ACES, new CategoryScore(CATEGORIES.ACES) },
    //    { CATEGORIES.TWOS, new CategoryScore(CATEGORIES.TWOS) },
    //    { CATEGORIES.THREES, new CategoryScore(CATEGORIES.THREES) }
    //};    

    public ScoreBoardPanelViewModel()
    {
        WeakReferenceMessenger.Default.Register<DiceRollMessage>(this, DiceRollMessageHandler);
    }

    private void DiceRollMessageHandler(object recipient, DiceRollMessage msg)
    {
        var Payload = msg.Value;

        foreach (var item in ScoreBoard.Scores.Where(s => !s.Assigned))
        {
            ScoreBoard.Scores[(int)item.Category].Score = Payload.DiceScores[item.Category];
            //item.Score = Payload.DiceScores[item.Category];
        }
        //Scores = ScoreBoard.Scores.ToDictionary(s => s.Category, s => s.Score);

        //ScoreBoardDict[CATEGORIES.ACES].Score = 5;

        //ScoreBoardDict.Remove(CATEGORIES.ACES);
        //ScoreBoardDict.Add(CATEGORIES.ACES, ScoreBoard.Scores[(int)CATEGORIES.ACES]);

        DiceRollNotified = true;
    }

    ~ScoreBoardPanelViewModel()
    {
        WeakReferenceMessenger.Default.Unregister<DiceRollMessage>(this);
    }

    [RelayCommand(CanExecute = nameof(CanSelectCategory))]
    public async Task SelectCategory(CATEGORIES selectedCategory)
    {
        if (!DiceRollNotified) return;

        var categoryScore = ScoreBoard.Scores.First(c => c.Category == selectedCategory);
        //categoryScore.AssignScore(Scores[selectedCategory]);
        WeakReferenceMessenger.Default.Send(new CategorySelectMessage(selectedCategory, isGameOVer()));

        DiceRollNotified = false;

        // ****************************

        //var score = PreviousCategory != selectedCategory ? DiceSet.GetScore(selectedCategory) : 0;
        //ScoreBoard.SelectCategory(selectedCategory, PreviousCategory, score);
        //PreviousCategory = selectedCategory; // != PreviousCategory ? selectedCategory : CATEGORIES.NONE;

        //var gameOver = ScoreBoard.IsGameOver();

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
        //System.Diagnostics.Debug.WriteLine("{0} {1} {2}",
        //    ScoreBoard.First(s => s.Category == category).Category,
        //    ScoreBoard.First(s => s.Category == category).Assigned,
        //    DiceRollNotified);


        return !ScoreBoard.Scores.First(s => s.Category == category).Assigned; // && DiceRollNotified;
    }

    private bool isGameOVer()
    {
        return false;
    }
}