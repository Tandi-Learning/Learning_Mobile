using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahtzee.Services;

namespace Yahtzee.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    protected bool _gameInProgress;

    public BaseViewModel()
    {
        // TODO: get this from the database
        GameInProgress = false;
    }
}
