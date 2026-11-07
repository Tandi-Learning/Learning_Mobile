using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Yahtzee.Data.Repositories;

namespace Yahtzee.PageModels
{
    /// <summary>
    /// Backs the home page and nothing else: the way into a new game, the saved one, and the score history.
    /// </summary>
    public partial class MainPageModel : ObservableObject
    {
        private readonly GameStateRepository _gameStateRepository;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ResumeGameCommand))]
        private bool _hasResumableGame;

        public MainPageModel(GameStateRepository gameStateRepository)
        {
            _gameStateRepository = gameStateRepository;
        }

        [RelayCommand]
        private async Task NewGame()
        {
            // Discard the saved board before navigating: the game page restores whatever it finds, so a
            // finished or abandoned game must not come back as a new one.
            await _gameStateRepository.ClearAsync();
            await Shell.Current.GoToAsync("game");
        }

        [RelayCommand(CanExecute = nameof(CanResumeGame))]
        private Task ResumeGame()
            // Nothing to restore here: the game page loads the saved board when it appears.
            => Shell.Current.GoToAsync("game");

        [RelayCommand]
        private Task ScoreHistory()
            => Shell.Current.GoToAsync("scorehistory");

        [RelayCommand]
        private void Exit()
        {

        }

        private bool CanResumeGame() => HasResumableGame;

        [RelayCommand]
        private async Task Appearing()
        {
            HasResumableGame = await _gameStateRepository.IsGameInProgressAsync();
        }
    }
}
