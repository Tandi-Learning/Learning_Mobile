using CommunityToolkit.Mvvm.ComponentModel;

namespace Yahtzee.Models;

public partial class Dice : ObservableObject
{
    public Dice(DICE_POSITION position)
    {
        Position = position;
    }

    [ObservableProperty]
    public DICE_POSITION _position = DICE_POSITION.NONE;

    //[ObservableProperty]
    //public int _position = 0;

    [ObservableProperty]
    public int _index = 0;

    [ObservableProperty]
    public bool _hold = false;

    [ObservableProperty]
    public DICE_STATE _state = DICE_STATE.OPEN;

    [ObservableProperty]
    public bool _keep = false;

    [ObservableProperty]
    public int _value = 0;

    [ObservableProperty]
    public string _image = GetDiceImage(0);

    [ObservableProperty]
    public string _backgroundColor = String.Empty;

    public void Reset()  
    {
        Hold = false;
        SetValue(0);
        Image = GetDiceImage(0);
    }

    public void SetValue(int value)
    {
        Value = value;
        Image = GetDiceImage(value);
    }

    // _keep observable property changed callback
    partial void OnKeepChanged(bool value)
    {
        BackgroundColor = value ? CONSTANTS.RED_BGCOLOR : CONSTANTS.TRANSPARENT_BGCOLOR;
    }

    private static string GetDiceImage(int value)
    {
        return value switch
        {
            1 => CONSTANTS.DICE_1_IMAGE,
            2 => CONSTANTS.DICE_2_IMAGE,
            3 => CONSTANTS.DICE_3_IMAGE,
            4 => CONSTANTS.DICE_4_IMAGE,
            5 => CONSTANTS.DICE_5_IMAGE,
            6 => CONSTANTS.DICE_6_IMAGE,
            _ => CONSTANTS.DICE_BLANK_IMAGE
        };
    }

    public void Roll()
    {
        Random random = new Random();
        Value = random.Next(1, 7);
        Image = GetDiceImage(Value);
    }

    public void Toggle()
    {
        State = State == DICE_STATE.HOLD ? DICE_STATE.OPEN: DICE_STATE.HOLD;
    }
}
