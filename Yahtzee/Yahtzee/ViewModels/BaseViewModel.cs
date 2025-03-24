using CommunityToolkit.Mvvm.ComponentModel;

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
