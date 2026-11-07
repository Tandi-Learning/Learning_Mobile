using System.ComponentModel;
using System.Globalization;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Yahtzee.Data.Repositories;
using Yahtzee.Models;
using Yahtzee.Scoring;
using Yahtzee.Services;

namespace Yahtzee.PageModels;

/// <summary>
/// Backs <see cref="Pages.GamePage"/>.
/// <para>
/// The page's bindings contract: every dynamic element on the page points at a member here. Rolling,
/// holding and committing a category are live, and each of them is written to the single state row so the
/// game survives leaving the page.
/// </para>
/// </summary>
public partial class GamePageModel : ObservableObject
{
    public const int TotalRounds = 13;
    public const int RollsPerTurn = 3;
    public const int DiceCount = 5;

    private readonly GameStateRepository _gameStateRepository;
    private readonly ModalErrorHandler _errorHandler;
    private bool _isLoaded;

    private readonly ScoreCategory _upperBonus;
    private readonly ScoreCategory _upperTotal;
    private readonly ScoreCategory _lowerTotal;
    private readonly ScoreCategory _yahtzee;
    private readonly ScoreCategory _yahtzeeBonus;

    private readonly ResultRepository _resultRepository;

    public GamePageModel(GameStateRepository gameStateRepository, ResultRepository resultRepository, ModalErrorHandler errorHandler)
    {
        _gameStateRepository = gameStateRepository;
        _resultRepository = resultRepository;
        _errorHandler = errorHandler;

        // Dice start unrolled (Value 0, drawn as ☐) until the first roll gives them faces.
        Dice = [.. Enumerable.Range(0, DiceCount).Select(_ => new Die())];
        Scorecard = CreateScorecard();
        UpperSection = [.. Scorecard.Where(category => category.IsUpper)];
        LowerSection = [.. Scorecard.Where(category => !category.IsUpper)];
        GrandTotal = new("Grand Total", isUpper: false, isTotal: true);

        // The derived rows are found once and then written by UpdateSectionTotals; the grand total follows
        // the two section totals rather than being written here.
        _upperBonus = UpperSection.First(category => category.IsBonus);
        _upperTotal = UpperSection.First(category => category.IsTotal);
        _lowerTotal = LowerSection.First(category => category.IsTotal);
        _yahtzee = LowerSection.First(category => category.Category == ScoringCategory.Yahtzee);
        _yahtzeeBonus = LowerSection.First(category => category.IsBonus);
        _upperTotal.PropertyChanged += OnSectionTotalChanged;
        _lowerTotal.PropertyChanged += OnSectionTotalChanged;
        UpdateGrandTotal();
    }

    public IReadOnlyList<Die> Dice { get; }

    /// <summary>
    /// The two scoring sections: 13 categories plus the derived bonus and section totals, in
    /// scorecard order. The columns are filtered views of it.
    /// </summary>
    public IReadOnlyList<ScoreCategory> Scorecard { get; }

    /// <summary>Ones–Sixes and the upper bonus: the left-hand scorecard column.</summary>
    public IReadOnlyList<ScoreCategory> UpperSection { get; }

    /// <summary>The combinations: the right-hand scorecard column.</summary>
    public IReadOnlyList<ScoreCategory> LowerSection { get; }

    /// <summary>
    /// The bottom line: both sections added together. It spans both columns, so it belongs to
    /// neither <see cref="UpperSection"/> nor <see cref="LowerSection"/>.
    /// </summary>
    public ScoreCategory GrandTotal { get; }

    /// <summary>
    /// The round the player is on: committed categories plus one. It is the clock the ending is read from,
    /// and it is persisted, so restoring the round restores the ending with it.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsGameOver))]
    [NotifyPropertyChangedFor(nameof(CanRoll))]
    [NotifyCanExecuteChangedFor(nameof(RollCommand))]
    [NotifyCanExecuteChangedFor(nameof(PlayAgainCommand))]
    private int _round = 1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotTopTen))]
    private bool _isTopTen;

    /// <summary>True once the thirteenth category has been committed. Derived rather than stored: the round already
    /// says it, and a restored round says it without a second value to keep in step.</summary>
    public bool IsGameOver => Round > TotalRounds;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanRoll))]
    [NotifyPropertyChangedFor(nameof(IsFirstRollAvailable))]
    [NotifyPropertyChangedFor(nameof(IsSecondRollAvailable))]
    [NotifyPropertyChangedFor(nameof(IsThirdRollAvailable))]
    [NotifyCanExecuteChangedFor(nameof(RollCommand))]
    private int _rollsRemaining = RollsPerTurn;

    public bool CanRoll => RollsRemaining > 0 && !IsGameOver;

    // One light per roll in the turn: it goes out as that roll is used, so the player can see how
    // many are left without reading a number.
    public bool IsFirstRollAvailable => RollsRemaining >= 1;

    public bool IsSecondRollAvailable => RollsRemaining >= 2;

    public bool IsThirdRollAvailable => RollsRemaining >= 3;

    /// <summary>
    /// A category can only be scored off a real roll, and the rules reject a blank die (value 0). Asked of
    /// the dice rather than of the roll counter: the counter only says how many rolls are left, and a
    /// restored row can carry a counter that disagrees with the table.
    /// </summary>
    private bool HasRolled => Dice.All(die => die.Value > 0);

    /// <summary>
    /// Restores the saved board the first time the page appears. A page is created per navigation, so this
    /// runs once for each visit rather than on every re-appearance.
    /// </summary>
    [RelayCommand]
    private async Task Appearing()
    {
        if (_isLoaded)
        {
            return;
        }

        _isLoaded = true;

        try
        {
            await LoadAsync();
        }
        catch (Exception exception)
        {
            _errorHandler.HandleError(exception);
        }
    }

    /// <summary>
    /// Raised once the dice have been re-faced, so the view can show the roll. The model says the dice
    /// changed; how that looks is the view's business.
    /// </summary>
    public event EventHandler? Rolled;

    [RelayCommand(CanExecute = nameof(CanRoll))]
    private async Task Roll()
    {
        // Held dice keep the face they already show, so a second roll only disturbs the rest.
        foreach (Die die in Dice)
        {
            if (!die.IsHeld)
            {
                die.Value = Random.Shared.Next(1, Die.FaceCount + 1);
            }
        }

        RollsRemaining--;
        UpdatePreviews();
        Rolled?.Invoke(this, EventArgs.Empty);

        await SaveAsync();
    }

    [RelayCommand]
    private async Task ToggleHold(Die? die)
    {
        // A hold only means something once there are faces to keep: a blank die held before the first
        // roll would stay 0 through the roll, which the scoring rules reject. Blank dice also cover
        // the game-over turn, where rolling is over anyway.
        if (die is null || !HasRolled)
        {
            return;
        }

        die.IsHeld = !die.IsHeld;

        await SaveAsync();
    }

    /// <summary>
    /// Tapping a row is the whole selection: it scores the category for the dice on the table and ends
    /// the turn. Rows that are already used, or derived from other rows, are ignored.
    /// </summary>
    [RelayCommand]
    private async Task ScoreRow(ScoreCategory? row)
    {
        if (row?.Category is not { } category || !row.CanScore || !HasRolled)
        {
            return;
        }

        int[] values = [.. Dice.Select(die => die.Value)];
        bool yahtzeeBoxHoldsFifty = _yahtzee.Score == ScoringRules.YahtzeePoints;

        row.Score = ScoringRules.Score(category, values, yahtzeeBoxHoldsFifty);
        row.IsAvailable = false;

        // A five of a kind taken after the Yahtzee row already holds 50 is worth its 100-point bonus. It
        // is awarded when the player commits the turn, so one roll can only pay it once.
        if (yahtzeeBoxHoldsFifty && ScoringRules.IsFiveOfAKind(values))
        {
            _yahtzeeBonus.Score = (_yahtzeeBonus.Score ?? 0) + ScoringRules.YahtzeeBonus(yahtzeeBoxHoldsFifty);
        }

        UpdateSectionTotals();

        // One round is one committed category, so the clock ticks here rather than in the reset below:
        // a fresh game clears the dice without that counting as a turn.
        Round++;
        StartNewTurn();

        await SaveAsync();

        if (IsGameOver)
        {
            int grandTotal = GrandTotal.Score!.Value;
            IsTopTen = await _resultRepository.IsInTopNAsync(grandTotal, ResultRepository.LeaderboardSize);
            await ShowEndGameDialog();
        }
    }

    /// <summary>
    /// A section total is itself a summed row, so only a new <see cref="ScoreCategory.Score"/> can move
    /// the grand total — availability changes never do.
    /// </summary>
    private void OnSectionTotalChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ScoreCategory.Score))
        {
            UpdateGrandTotal();
        }
    }

    private void UpdateGrandTotal() =>
        GrandTotal.Score = (_upperTotal.Score ?? 0) + (_lowerTotal.Score ?? 0);

    /// <summary>
    /// Re-sums what the scorecard currently holds. The upper total includes the bonus so that the grand
    /// total stays the two totals added together; the Yahtzee bonus counts toward the lower total.
    /// </summary>
    private void UpdateSectionTotals()
    {
        int upperFaces = UpperSection.Where(row => !row.IsDerived).Sum(row => row.Score ?? 0);
        _upperBonus.Score = ScoringRules.UpperBonus(upperFaces);
        _upperTotal.Score = upperFaces + (_upperBonus.Score ?? 0);

        // Every lower row except the running total itself: the Yahtzee bonus is scored too.
        _lowerTotal.Score = LowerSection.Where(row => !row.IsTotal).Sum(row => row.Score ?? 0);
    }

    /// <summary>
    /// Shows what each open row would pay for the dice on the table. Cleared at the start of a turn,
    /// where there is nothing to score yet.
    /// </summary>
    private void UpdatePreviews()
    {
        int[] values = [.. Dice.Select(die => die.Value)];
        bool yahtzeeBoxHoldsFifty = _yahtzee.Score == ScoringRules.YahtzeePoints;

        foreach (ScoreCategory row in Scorecard)
        {
            row.Preview = row.Category is { } category && row.IsAvailable && HasRolled
                ? ScoringRules.Score(category, values, yahtzeeBoxHoldsFifty)
                : null;
        }
    }

    /// <summary>
    /// Hands the dice back for the next turn: nothing held, three rolls. The round is not touched here:
    /// it counts committed categories, and a reset is not a turn.
    /// </summary>
    private void StartNewTurn()
    {
        foreach (Die die in Dice)
        {
            die.IsHeld = false;
            die.Value = 0;
        }

        RollsRemaining = RollsPerTurn;
        UpdatePreviews();
    }

    /// <summary>
    /// Puts the board back to how a game opens: first round, every category open, dice blank. Nothing is
    /// saved here — the caller decides whether this is a new game worth writing.
    /// </summary>
    private void StartNewGame()
    {
        Round = 1;

        foreach (ScoreCategory row in Scorecard)
        {
            row.Score = null;
            row.IsAvailable = true;
        }

        StartNewTurn();
        UpdateSectionTotals();

        IsTopTen = false;
    }

    [ObservableProperty]
    private string _playerName = string.Empty;

    /// <summary>Inverse of <see cref="IsTopTen"/> for XAML visibility toggling inside the popup.</summary>
    public bool IsNotTopTen => !IsTopTen;

    /// <summary>
    /// Opens the end-game modal dialog after a game finishes. The dialog is the only UI for the finished-game card.
    /// </summary>
    private async Task ShowEndGameDialog()
    {
        // Pushed as a modal rather than navigated to as a Shell route: a modal sits outside the navigation
        // stack, so the back button can never return to a finished game's card. It binds this same model, so
        // the dialog edits the game in progress rather than a copy.
        await Shell.Current.Navigation.PushModalAsync(new Yahtzee.Pages.GameEndPopupPage { BindingContext = this });
    }

    /// <summary>
    /// A top-10 finish is recorded whenever the player leaves the end-game card, whether by starting another game
    /// or by quitting. A blank name becomes "No name". PlayerName is cleared after writing the result.
    /// This is called by the end-game dialog rather than by an in-page frame, since there is no in-page frame anymore.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanPlayAgain))]
    private async Task PlayAgain()
    {
        await SaveResultIfTopTenAsync();
        await _gameStateRepository.ClearAsync();
        StartNewGame();
    }

    private bool CanPlayAgain => IsGameOver;

    [RelayCommand]
    private async Task Quit()
    {
        await SaveResultIfTopTenAsync();
        PlayerName = string.Empty;
        await Shell.Current.GoToAsync("//main");
    }

    private async Task SaveResultIfTopTenAsync()
    {
        if (!IsTopTen)
        {
            return;
        }

        string name = string.IsNullOrWhiteSpace(PlayerName) ? "No name" : PlayerName.Trim();
        GameResult result = new GameResult
        {
            Name = name,
            Score = GrandTotal.Score.Value,
            FinishedAtTicks = DateTime.Now.Ticks,
        };
        await _resultRepository.InsertTopAsync(result, ResultRepository.LeaderboardSize);

        PlayerName = string.Empty;
    }

    /// <summary>
    /// Called by the end-game dialog when the player chooses Play Again. Saves the result if needed, restarts the
    /// game on the shared model, then returns to the game page so the new board is visible.
    /// </summary>
    [RelayCommand]
    public async Task PlayAgainDialog()
    {
        await PlayAgain();
        await Shell.Current.Navigation.PopModalAsync();
    }

    /// <summary>
    /// Called by the end-game dialog when the player chooses Quit. Saves the result if needed, clears the name, then
    /// returns to the main page.
    /// </summary>
    [RelayCommand]
    public async Task QuitDialog()
    {
        // Close the dialog before leaving: a modal draws above the Shell, so navigating underneath it
        // would leave the card on screen over the main page.
        await Shell.Current.Navigation.PopModalAsync();
        await Quit();
    }

    /// <summary>
    /// Restores the board from the saved row, or opens a fresh one when there is nothing to resume. Only
    /// what the player chose comes back; the totals and the bonus are re-summed from the scores.
    /// </summary>
    private async Task LoadAsync()
    {
        GameState? state = await _gameStateRepository.GetAsync();

        // A game that was never started, or one that has just been played out, has no board to restore.
        if (state is not { IsInProgress: true })
        {
            StartNewGame();
            return;
        }

        // Clamped rather than trusted: the stored row is only as good as the last version that wrote it.
        Round = Math.Clamp(state.Round, 1, TotalRounds + 1);
        RollsRemaining = Math.Clamp(state.RollsRemaining, 0, RollsPerTurn);
        ApplyDice(state.Dice, state.Holds);
        ApplyScores(state.Scores);

        // A saved board can be unplayable: no rolls left and nothing rolled, so there is neither a table
        // to score nor a roll to take. Handing the turn back beats locking the player out of both.
        if (RollsRemaining == 0 && !HasRolled)
        {
            StartNewTurn();
        }

        UpdateSectionTotals();
        UpdatePreviews();
    }

    private async Task SaveAsync()
    {
        try
        {
            await _gameStateRepository.SaveAsync(BuildState());
        }
        catch (Exception exception)
        {
            // Losing the save costs the player one turn on their next visit, so it is worth saying out
            // loud rather than swallowing.
            _errorHandler.HandleError(exception);
        }
    }

    /// <summary>
    /// The row for the board on screen. A game that has just ended is written as no longer in progress,
    /// which is what keeps a finished board off the dashboard's resume path.
    /// </summary>
    private GameState BuildState() => new()
    {
        IsInProgress = !IsGameOver,
        Round = Round,
        RollsRemaining = RollsRemaining,
        Dice = string.Join(",", Dice.Select(die => die.Value.ToString(CultureInfo.InvariantCulture))),
        Holds = string.Join(",", Dice.Select(die => die.IsHeld ? 1 : 0)),
        Scores = PackScores(),
    };

    /// <summary>
    /// The 13 committed scores, blank where a category is still open. Keyed by category rather than by row
    /// position, so rearranging the scorecard cannot shuffle the saved scores.
    /// </summary>
    private string PackScores()
    {
        ScoringCategory[] categories = Enum.GetValues<ScoringCategory>();
        string[] scores = new string[categories.Length];

        for (int index = 0; index < categories.Length; index++)
        {
            ScoreCategory? row = Scorecard.FirstOrDefault(candidate => candidate.Category == categories[index]);
            scores[index] = row?.Score?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        }

        return string.Join(",", scores);
    }

    /// <summary>
    /// Restores the committed scores. An entry that is missing or unreadable counts as an open row, so a
    /// damaged or older row degrades to a playable board instead of failing the load.
    /// </summary>
    private void ApplyScores(string? packed)
    {
        ScoringCategory[] categories = Enum.GetValues<ScoringCategory>();
        string[] parts = (packed ?? string.Empty).Split(',');

        for (int index = 0; index < categories.Length; index++)
        {
            ScoreCategory? row = Scorecard.FirstOrDefault(candidate => candidate.Category == categories[index]);
            if (row is null)
            {
                continue;
            }

            if (index >= parts.Length ||
                !int.TryParse(parts[index], NumberStyles.Integer, CultureInfo.InvariantCulture, out int score))
            {
                row.Score = null;
                row.IsAvailable = true;
                continue;
            }

            row.Score = score;
            row.IsAvailable = false;
        }
    }

    /// <summary>
    /// Restores the faces and the holds. A missing or damaged entry comes back as an unrolled, unheld die.
    /// </summary>
    private void ApplyDice(string? packedValues, string? packedHolds)
    {
        int[] values = ParseDice(packedValues);
        int[] holds = ParseDice(packedHolds);

        for (int index = 0; index < Dice.Count; index++)
        {
            Dice[index].Value = Math.Clamp(values[index], 0, Die.FaceCount);
            Dice[index].IsHeld = holds[index] != 0;
        }
    }

    /// <summary>One comma-separated entry per die; anything missing or unreadable reads as 0.</summary>
    private static int[] ParseDice(string? packed)
    {
        string[] parts = (packed ?? string.Empty).Split(',');
        int[] entries = new int[DiceCount];

        for (int index = 0; index < DiceCount && index < parts.Length; index++)
        {
            _ = int.TryParse(parts[index], NumberStyles.Integer, CultureInfo.InvariantCulture, out entries[index]);
        }

        return entries;
    }

    /// <summary>
    /// The standard 13 categories plus the derived rows that sit inside a column (the upper bonus and
    /// the two section totals). This list is fixed for the whole game, so it is built once; the
    /// <see cref="ScoreCategory"/> instances themselves raise change notifications as scores land.
    /// Order is display order, and the two columns are filtered views of it. The grand total spans
    /// both columns and is not part of this list.
    /// </summary>
    private static List<ScoreCategory> CreateScorecard() =>
    [
        new("Ones", isUpper: true, category: ScoringCategory.Ones),
        new("Twos", isUpper: true, category: ScoringCategory.Twos),
        new("Threes", isUpper: true, category: ScoringCategory.Threes),
        new("Fours", isUpper: true, category: ScoringCategory.Fours),
        new("Fives", isUpper: true, category: ScoringCategory.Fives),
        new("Sixes", isUpper: true, category: ScoringCategory.Sixes),
        new("Upper Bonus (63+)", isUpper: true, isBonus: true),
        new("Upper Total", isUpper: true, isTotal: true),
        new("Three of a Kind", isUpper: false, category: ScoringCategory.ThreeOfAKind),
        new("Four of a Kind", isUpper: false, category: ScoringCategory.FourOfAKind),
        new("Full House", isUpper: false, category: ScoringCategory.FullHouse),
        new("Small Straight", isUpper: false, category: ScoringCategory.SmallStraight),
        new("Large Straight", isUpper: false, category: ScoringCategory.LargeStraight),
        new("Yahtzee", isUpper: false, category: ScoringCategory.Yahtzee),
        new("Chance", isUpper: false, category: ScoringCategory.Chance),
        new("Yahtzee Bonus", isUpper: false, isBonus: true),
        new("Lower Total", isUpper: false, isTotal: true),
    ];
}
