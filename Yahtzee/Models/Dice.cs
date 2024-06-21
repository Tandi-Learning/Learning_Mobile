using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;

namespace Yahtzee.Models;

public partial class RollIndicator : ObservableObject
{
    [ObservableProperty]
    public ROLL_COUNT _count;

    [ObservableProperty]
    public string _image = "green_led.png";
}

public partial class Dice : ObservableObject
{
    public Dice()
    { }

    public Dice(DICE_POSITION position)
    {
        Position = position;
    }

    [ObservableProperty]
    public DICE_POSITION _position;

    [ObservableProperty]
    public bool _hold = false;

    [ObservableProperty]
    public int _value = 0;

    [ObservableProperty]
    public string _image = "dice_blank.png";

    [ObservableProperty]
    public string _backgroundColor = "Transparent";

    partial void OnHoldChanged(bool value)
    {
        BackgroundColor = value ? "Red" : "Transparent";
    }
}
