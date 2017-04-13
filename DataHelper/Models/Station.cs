using System;
using System.Collections.Generic;

namespace DataHelper.Models
{
    public class Station
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public int PlaylistServiceId { get; set; }

        public int Rating { get; set; }

        public DateTime? LastUpdateTime { get; set; }
        
        public virtual ICollection<Track> Tracks { get; set; }

        public virtual PlaylistService PlaylistService { get; set; }
 
    }
}