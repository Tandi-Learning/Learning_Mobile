using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Yahtzee.Models;
using Yahtzee.ViewModels;


namespace Yahtzee.Controls;

public partial class DicePanelView : ContentView
{
    public DicePanelView()
    {
        InitializeComponent();
        BindingContext = new DicePanelViewModel();
    }
}