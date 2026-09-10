using CommunityToolkit.Maui.Layouts;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;

namespace Yahtzee.Models;

//public partial class Dice : ObservableObject
//{
//    private bool hold = false;

//    public Dice()
//    { }

//    public Dice(DICE_POSITION position)
//    {
//        Position = position;
//        var a = StateContainer.CurrentStateProperty;
//    }

//    [ObservableProperty]
//    public DICE_POSITION _position;

//    [ObservableProperty]
//    public int _ordinal = 0;

//    [ObservableProperty]
//    public bool _holdx = false;

//    [ObservableProperty]
//    public int _value = 0;

//    [ObservableProperty]
//    public string _image = "dice_blank.png";

//    [ObservableProperty]
//    public string _backgroundColor = "Transparent";

//    partial void OnHoldChanged(bool value)
//    {
//        BackgroundColor = value ? "Red" : "Transparent";
//    }
//}
