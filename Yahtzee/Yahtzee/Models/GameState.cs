using SQLite;

namespace Yahtzee.Models;

[Table("GameState")]
public class GameState
{
    [PrimaryKey]
    public int ID { get; set; } = 1;

    public bool IsInProgress { get; set; }
}
