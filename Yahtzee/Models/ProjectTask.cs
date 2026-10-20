using System.Text.Json.Serialization;
using SQLite;

namespace Yahtzee.Models
{
    [Table("Task")]
    public class ProjectTask
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }

        [JsonIgnore]
        public int ProjectID { get; set; }
    }
}