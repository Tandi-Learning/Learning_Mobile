using CommunityToolkit.Mvvm.Input;
using Yahtzee.Messaging;
using Yahtzee.Models;
using Yahtzee.Services;
using Yahtzee.ViewModels;

namespace Yahtzee.Controls;

public partial class ScoreBoardPanelView : ContentView
{
    public ScoreBoardPanelView()
	{
		InitializeComponent();
		BindingContext = new ScoreBoardPanelViewModel();
    }
}