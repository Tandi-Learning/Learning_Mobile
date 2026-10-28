using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Yahtzee.Models;

/// <summary>
/// One row of the scorecard. <see cref="Score"/> is null until the category has been filled in,
/// which is also what drives the "—" placeholder. Rows are either scored by the player or derived
/// (the upper bonus and the two totals) — see <see cref="IsDerived"/>.
/// </summary>
public partial class ScoreCategory : ObservableObject
{
    public ScoreCategory(string name, bool isUpper, bool isBonus = false, bool isTotal = false)
    {
        _name = name;
        IsUpper = isUpper;
        IsBonus = isBonus;
        IsTotal = isTotal;
    }

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasScore))]
    [NotifyPropertyChangedFor(nameof(DisplayScore))]
    private int? _score;

    /// <summary>False once the category is used, or when the current dice cannot score it.</summary>
    [ObservableProperty]
    private bool _isAvailable = true;

    /// <summary>Upper section (Ones–Sixes) or lower section (the combinations).</summary>
    public bool IsUpper { get; }

    /// <summary>True for the 35-point upper-section bonus row, which is derived, not scored.</summary>
    public bool IsBonus { get; }

    /// <summary>True for the upper and lower running totals, which are summed, not scored.</summary>
    public bool IsTotal { get; }

    /// <summary>True for any row worked out from the others, so the player never fills it in.</summary>
    public bool IsDerived => IsBonus || IsTotal;

    public bool HasScore => Score.HasValue;

    /// <summary>A total is the sum of an empty section, so it starts at 0; scored rows stay blank.</summary>
    public string DisplayScore => Score?.ToString(CultureInfo.CurrentCulture) ?? (IsTotal ? "0" : "—");
}
