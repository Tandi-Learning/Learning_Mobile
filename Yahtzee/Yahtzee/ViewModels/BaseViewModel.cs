using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Yahtzee.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    protected bool _gameInProgress;

    public bool ChildCanExecute()
    {
        throw new NotImplementedException();
    }

    public BaseViewModel()
    {
        // TODO: get this from the database
    }


}
