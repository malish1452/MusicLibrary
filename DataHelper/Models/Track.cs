using System;
using System.Collections.Generic;

namespace DataHelper.Models
{
    public class Track
    {
        public int Id { get; set; }
        public string MixedName { get; set; }
        public string Name { get; set; }
        public int? MusicianId { get; set; }
        public TimeSpan? Duration { get; set; }
        public int? BitRate { get; set; }
        public string Path { get; set; }
        public int StatusId { get; set; }
        public virtual Musician Musician { get; set; }
      //  public virtual TrackStatus TrackStatus { get; set; }
        public int TrackScore { get; set; }

        public virtual ICollection<Station> Stations { get; set; }
        public virtual ICollection<MixedNames> MixedNames { get; set; }

    }
}