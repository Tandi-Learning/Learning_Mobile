namespace Yahtzee.Models
{
    using SQLite;

    public class ProjectsTags
    {
        [Ignore]
        public int ID { get; set; }

        [Indexed(Name = "ProjectsTags_Unique", Order = 1, Unique = true)]
        public int ProjectID { get; set; }

        [Indexed(Name = "ProjectsTags_Unique", Order = 2, Unique = true)]
        public int TagID { get; set; }
    }
}