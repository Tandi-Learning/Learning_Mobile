using CommunityToolkit.Maui.Core.Extensions;
using System.Text.Json.Serialization;
using SQLite;

namespace Yahtzee.Models
{
    public class Tag
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Color { get; set; } = "#FF0000";

        [JsonIgnore]
        [Ignore]
        public Brush ColorBrush
        {
            get
            {
                return new SolidColorBrush(Microsoft.Maui.Graphics.Color.FromArgb(Color));
            }
        }

        [JsonIgnore]
        [Ignore]
        public Color DisplayColor
        {
            get
            {
                return Microsoft.Maui.Graphics.Color.FromArgb(Color);
            }
        }

        [JsonIgnore]
        [Ignore]
        public Color DisplayDarkColor
        {
            get
            {
                return DisplayColor.WithBlackKey(0.8);
            }
        }

        [JsonIgnore]
        [Ignore]
        public Color DisplayLightColor
        {
            get
            {
                return DisplayColor.WithBlackKey(0.2);
            }
        }

        [JsonIgnore]
        [Ignore]
        public bool IsSelected { get; set; }
    }
}