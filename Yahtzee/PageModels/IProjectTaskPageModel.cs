using CommunityToolkit.Mvvm.Input;
using Yahtzee.Models;

namespace Yahtzee.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}