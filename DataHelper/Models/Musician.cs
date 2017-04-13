using System.Collections.Generic;

namespace DataHelper.Models
{
    public class Musician

    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Score { get; set; }

        public virtual ICollection<Track> Tracks { get; set; } 
    }
}