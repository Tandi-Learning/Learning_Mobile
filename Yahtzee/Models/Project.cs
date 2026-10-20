using System.Text.Json.Serialization;
using SQLite;

namespace Yahtzee.Models
{
    public class Project
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;

        [JsonIgnore]
        public int CategoryID { get; set; }

        [Ignore]
        public Category? Category { get; set; }

        [Ignore]
        public List<ProjectTask> Tasks { get; set; } = [];

        [Ignore]
        public List<Tag> Tags { get; set; } = [];

        [Ignore]
        public string AccessibilityDescription
        {
            get { return $"{Name} Project. {Description}"; }
        }

        public override string ToString() => $"{Name}";
    }

    public class ProjectsJson
    {
        public List<Project> Projects { get; set; } = [];
    }
}