namespace DataHelper.Models
{
    public class MixedNames
    {
        public int Id { get; set; }
        public int TrackId { get; set; }

        public string MixedName { get; set; }

        public virtual Track Track { get; set; }

    }
}
