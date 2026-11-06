using SQLite;

namespace Yahtzee.Models;

[Table("GameResult")]
public class GameResult
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    /// <summary>Player-supplied name, max 10 characters on the entry; blank becomes "No name".</summary>
    [MaxLength(20)]
    public string Name { get; set; } = string.Empty;

    public int Score { get; set; }

    /// <summary>Local clock at the moment the game ended, stored as ticks so the column is a plain integer.</summary>
    public long FinishedAtTicks { get; set; }

    /// <summary>Local-date display of <see cref="FinishedAtTicks"/> for the score-history list.</summary>
    public string FinishedAtDisplay => new DateTime(FinishedAtTicks).ToString("yyyy-MM-dd HH:mm");
}
