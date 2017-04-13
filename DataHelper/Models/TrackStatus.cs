using System.Collections.Generic;

namespace DataHelper.Models
{
    public class TrackStatus
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }
    }
}