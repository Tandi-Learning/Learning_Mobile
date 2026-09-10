using CommunityToolkit.Maui.Layouts;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yahtzee.Models
{    
    public partial class RollIndicator : ObservableObject
    {
        [ObservableProperty]
        public ROLL_COUNT _count;

        [ObservableProperty]
        public string _image = "green_led.png";
    }

    public partial class RollState : ObservableObject
    {
        [ObservableProperty]
        public ROLL_COUNT _count;

        [ObservableProperty]
        public string _image = "green_led.png";
    }
}
