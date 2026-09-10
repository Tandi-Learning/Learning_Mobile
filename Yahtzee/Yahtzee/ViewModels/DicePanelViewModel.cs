
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using Yahtzee.Messaging;
using Yahtzee.Models;

namespace Yahtzee.ViewModels;

public partial class DicePanelViewModel : ObservableObject
{
    public DicePanelViewModel()
    {
        WeakReferenceMessenger.Default.Register<CategorySelectMessage>(this, CategorySelectMessageHandler);
    }

    //[ObservableProperty]
    //public ObservableCollection<string> _diceBackColor = new() { CONSTANTS.RED_BGCOLOR, CONSTANTS.RED_BGCOLOR, CONSTANTS.RED_BGCOLOR, CONSTANTS.RED_BGCOLOR, CONSTANTS.RED_BGCOLOR };

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RollDicesCommand))]
    public int _rollCount = 0;

    [ObservableProperty]
    public ObservableCollection<Dice> _dices =
    [
        new Dice(DICE_POSITION.FIRST),
        new Dice(DICE_POSITION.SECOND),
        new Dice(DICE_POSITION.THIRD),
        new Dice(DICE_POSITION.FOURTH),
        new Dice(DICE_POSITION.FIFTH)
    ];

    [ObservableProperty]
    public ObservableCollection<string> _countImages = new() { CONSTANTS.ON_IMAGE, CONSTANTS.ON_IMAGE, CONSTANTS.ON_IMAGE };

    [RelayCommand(CanExecute = nameof(CanRollDice))]
    public void RollDices()
    {
        bool lastScore = false;
        _countImages[RollCount] = CONSTANTS.OFF_IMAGE;
        RollCount++;

        foreach (var dice in Dices)
        {
            if (!dice.Hold)
                dice.Roll();
        }
        RollDiceNotify();
    }

    public bool CanRollDice()
    {
        return RollCount < 3;
    }

    [RelayCommand(CanExecute = nameof(CanToggleDice))]
    public void ToggleDice(DICE_POSITION position)
    {
        //var isHold = position switch
        //{
        //    DICE_POSITION.FIRST => DiceOneSelected = !DiceOneSelected,
        //    DICE_POSITION.SECOND => DiceTwoSelected = !DiceTwoSelected,
        //    DICE_POSITION.THIRD => DiceThreeSelected = !DiceThreeSelected,
        //    DICE_POSITION.FOURTH => DiceFourSelected = !DiceFourSelected,
        //    DICE_POSITION.FIFTH => DiceFiveSelected = !DiceFiveSelected,
        //};

        Dices[(int)position].Hold = !Dices[(int)position].Hold;
    }

    private bool CanToggleDice()
    {
        return RollCount > 0 && RollCount < 3;
    }

    private void RollDiceNotify()
    {
        var payload = new DiceRollPayload(Dices);
        WeakReferenceMessenger.Default.Send(new DiceRollMessage(payload));
    }

    private void Reset()
    {
        RollCount = 0;

        CountImages[0] = CONSTANTS.ON_IMAGE;
        CountImages[1] = CONSTANTS.ON_IMAGE;
        CountImages[2] = CONSTANTS.ON_IMAGE;

        foreach (var dice in Dices)
        {
            dice.Reset();
        }
    }

    private void CategorySelectMessageHandler(object recipient, CategorySelectMessage msg)
    {
        if (msg.GameOver)
        {

        } else Reset();
    }
}
