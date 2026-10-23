using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Yahtzee.Models;

namespace Yahtzee.PageModels;

/// <summary>
/// Backs <see cref="Pages.GamePage"/>.
/// <para>
/// This is the bindings contract for milestone M1: every dynamic element on the page points at a
/// member here, and the members return placeholder data. The rules that fill them in come next
/// (see PLAN.md), and doing it this way means the rules can be added without touching the XAML.
/// </para>
/// </summary>
public partial class GamePageModel : ObservableObject
{
    public const int TotalRounds = 13;
    public const int RollsPerTurn = 3;
    public const int DiceCount = 5;

    public GamePageModel()
    {
        Dice = [.. Enumerable.Range(1, DiceCount).Select(value => new Die { Value = value })];
        Scorecard = CreateScorecard();
        UpperSection = [.. Scorecard.Where(category => category.IsUpper)];
        LowerSection = [.. Scorecard.Where(category => !category.IsUpper)];
        GrandTotal = new("Grand Total", isUpper: false, isTotal: true);
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
    [NotifyCanExecuteChangedFor(nameof(RollCommand))]
    private int _rollsRemaining = RollsPerTurn;

    [ObservableProperty]
    private int _totalScore;

    public bool CanRoll => RollsRemaining > 0;

    public bool CanEndTurn => true;

    [RelayCommand(CanExecute = nameof(CanRoll))]
    private void Roll()
    {
        // M3: re-roll every die that is not held and decrement RollsRemaining.
    }

    [RelayCommand(CanExecute = nameof(CanEndTurn))]
    private void EndTurn()
    {
        // M3: score the chosen category, advance Round, reset holds and RollsRemaining.
    }

    [RelayCommand]
    private void ToggleHold(Die? die)
    {
        if (die is null)
        {
            return;
        }

        die.IsHeld = !die.IsHeld;
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
        new("Ones", isUpper: true),
        new("Twos", isUpper: true),
        new("Threes", isUpper: true),
        new("Fours", isUpper: true),
        new("Fives", isUpper: true),
        new("Sixes", isUpper: true),
        new("Upper Bonus (63+)", isUpper: true, isBonus: true),
        new("Upper Total", isUpper: true, isTotal: true),
        new("Three of a Kind", isUpper: false),
        new("Four of a Kind", isUpper: false),
        new("Full House", isUpper: false),
        new("Small Straight", isUpper: false),
        new("Large Straight", isUpper: false),
        new("Yahtzee", isUpper: false),
        new("Chance", isUpper: false),
        new("Yahtzee Bonus", isUpper: false, isBonus: true),
        new("Lower Total", isUpper: false, isTotal: true),
    ];
}
