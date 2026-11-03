using SQLite;

namespace Yahtzee.Models;

/// <summary>
/// The saved state of the one game that can be in progress: a single row, <see cref="ID"/> = 1. It is a
/// singleton, not a collection — do not add rows.
/// </summary>
/// <remarks>
/// Only what the player chose is stored: the dice, which of them are held, the rolls left in the turn, the
/// round, and the 13 committed scores. Everything derived — the section totals, the upper bonus, the Yahtzee
/// bonus's running total, the grand total — is recomputed from those scores when the game is loaded, so
/// none of it is persisted.
/// <para>
/// The list columns are comma-separated because the lists are short and fixed: five entries for the dice,
/// five for the holds, and thirteen for the scores. Blank means "still open", and 0 in a die means it has
/// not been rolled yet.
/// </para>
/// </remarks>
[Table("GameState")]
public class GameState
{
    [PrimaryKey]
    public int ID { get; set; } = 1;

    /// <summary>True while there is a board worth resuming on the dashboard.</summary>
    public bool IsInProgress { get; set; }

    /// <summary>The round the player is on, 1-based: committed categories plus one.</summary>
    public int Round { get; set; } = 1;

    /// <summary>Rolls left in the current turn, counting down from three.</summary>
    public int RollsRemaining { get; set; }

    /// <summary>The five faces in board order, 0 = not rolled yet.</summary>
    public string Dice { get; set; } = string.Empty;

    /// <summary>One entry per die: 1 = held, 0 = free to re-roll.</summary>
    public string Holds { get; set; } = string.Empty;

    /// <summary>
    /// The 13 committed scores in <see cref="Scoring.ScoringCategory"/> order, blank where the category is
    /// still open. Empty for a game that has not scored anything yet.
    /// </summary>
    public string Scores { get; set; } = string.Empty;
}
