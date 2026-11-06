using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Yahtzee.Data.Repositories;
using Yahtzee.Models;

namespace Yahtzee.PageModels;

public partial class ScoreHistoryPageModel : ObservableObject
{
    private readonly ResultRepository _resultRepository;

    [ObservableProperty]
    private IReadOnlyList<GameResult> _results = [];

}

// ReSharper disable once PartialTypeWithSinglePart
public partial class ScoreHistoryPageModel
{
    public ScoreHistoryPageModel(ResultRepository resultRepository)
    {
        _resultRepository = resultRepository;
    }

    [RelayCommand]
    private async Task Appearing()
    {
        Results = await _resultRepository.ListTopAsync(ResultRepository.LeaderboardSize);
    }
}
