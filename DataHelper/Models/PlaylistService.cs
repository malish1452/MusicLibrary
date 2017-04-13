using System.Collections.Generic;

namespace DataHelper.Models
{
    public class PlaylistService
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public virtual ICollection<Station> Stations { get; set; }
    }
}