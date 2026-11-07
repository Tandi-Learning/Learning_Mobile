using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Yahtzee.Scoring;

namespace Yahtzee.Models;

/// <summary>
/// One row of the scorecard. <see cref="Score"/> is null until the category has been filled in,
/// which is also what drives the "—" placeholder. Rows are either scored by the player or derived
/// (the upper bonus and the two totals) — see <see cref="IsDerived"/>.
/// </summary>
public partial class ScoreCategory : ObservableObject
{
    public ScoreCategory(
        string name,
        bool isUpper,
        ScoringCategory? category = null,
        bool isBonus = false,
        bool isTotal = false)
    {
        _name = name;
        Category = category;
        IsUpper = isUpper;
        IsBonus = isBonus;
        IsTotal = isTotal;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Description))]
    private string _name;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasScore))]
    [NotifyPropertyChangedFor(nameof(IsSpent))]
    [NotifyPropertyChangedFor(nameof(DisplayScore))]
    [NotifyPropertyChangedFor(nameof(Description))]
    private int? _score;

    /// <summary>False once the category is used, or when the current dice cannot score it.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanScore))]
    private bool _isAvailable = true;

    /// <summary>
    /// What this row would pay for the dice on the table right now, shown while the row is still open so
    /// the player is not choosing blind. Null for derived rows, used rows and before the turn's first roll.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayScore))]
    [NotifyPropertyChangedFor(nameof(Description))]
    private int? _preview;

    /// <summary>
    /// The rules category this row is scored with. Null on the derived rows (bonus and totals), which is
    /// what makes them untappable.
    /// </summary>
    public ScoringCategory? Category { get; }

    /// <summary>True while the player may still commit this row.</summary>
    public bool CanScore => Category.HasValue && IsAvailable;

    /// <summary>Upper section (Ones–Sixes) or lower section (the combinations).</summary>
    public bool IsUpper { get; }

    /// <summary>True for the 35-point upper-section bonus row, which is derived, not scored.</summary>
    public bool IsBonus { get; }

    /// <summary>True for the upper and lower running totals, which are summed, not scored.</summary>
    public bool IsTotal { get; }

    /// <summary>True for any row worked out from the others, so the player never fills it in.</summary>
    public bool IsDerived => IsBonus || IsTotal;

    public bool HasScore => Score.HasValue;

    /// <summary>
    /// True once a row the player filled in has been committed, which is when it reads as spent (greyed).
    /// Derived rows are excluded on purpose: the bonus and totals always carry a value and stay bold.
    /// </summary>
    public bool IsSpent => HasScore && !IsDerived;

    /// <summary>
    /// A total is the sum of an empty section, so it starts at 0; scored rows stay blank. An open row shows
    /// its <see cref="Preview"/> so the player can see what tapping it would score.
    /// </summary>
    public string DisplayScore =>
        (Score ?? Preview)?.ToString(CultureInfo.CurrentCulture) ?? (IsTotal ? "0" : "—");

    /// <summary>
    /// The row as one sentence for a screen reader, which otherwise reads <see cref="Name"/> and
    /// <see cref="DisplayScore"/> as two unrelated items and never mentions that the row can be tapped.
    /// </summary>
    public string Description => IsDerived
        ? $"{Name}, {DisplayScore}"
        : Score is { } score
            ? $"{Name}, scored {score}"
            : Preview is { } preview
                ? $"{Name}, scores {preview}, tap to commit"
                : $"{Name}, no score yet";
}
