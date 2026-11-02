using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Yahtzee.Models;
using Yahtzee.Scoring;

namespace Yahtzee.PageModels;

/// <summary>
/// Backs <see cref="Pages.GamePage"/>.
/// <para>
/// The page's bindings contract: every dynamic element on the page points at a member here. The dice
/// side and the scorecard are both live — rolling, holding and committing a category are M3 (see
/// PLAN.md).
/// </para>
/// </summary>
public partial class GamePageModel : ObservableObject
{
    public const int TotalRounds = 13;
    public const int RollsPerTurn = 3;
    public const int DiceCount = 5;

    private bool IsGameOver { get; set; }

    private readonly ScoreCategory _upperBonus;
    private readonly ScoreCategory _upperTotal;
    private readonly ScoreCategory _lowerTotal;
    private readonly ScoreCategory _yahtzee;
    private readonly ScoreCategory _yahtzeeBonus;

    public GamePageModel()
    {
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

    [ObservableProperty]
    private int _round = 1;

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
    /// A category can only be scored off a real roll: before the turn's first roll the dice are still
    /// blank (value 0), which the rules reject.
    /// </summary>
    private bool HasRolled => RollsRemaining < RollsPerTurn;

    [RelayCommand(CanExecute = nameof(CanRoll))]
    private void Roll()
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
    }

    [RelayCommand]
    private void ToggleHold(Die? die)
    {
        // A hold only means something once there are faces to keep: a blank die held before the first
        // roll would stay 0 through the roll, which the scoring rules reject. Blank dice also cover
        // the game-over turn, where rolling is over anyway.
        if (die is null || !HasRolled)
        {
            return;
        }

        die.IsHeld = !die.IsHeld;
    }

    /// <summary>
    /// Tapping a row is the whole selection: it scores the category for the dice on the table and ends
    /// the turn. Rows that are already used, or derived from other rows, are ignored.
    /// </summary>
    [RelayCommand]
    private void ScoreRow(ScoreCategory? row)
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
        //Round++;
        IsGameOver = ++Round > TotalRounds;
        StartNewTurn();
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
