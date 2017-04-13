using System.ComponentModel;
using System.Data.Entity;
using DataHelper.Models;

namespace DataHelper.DBWork
{
    public class MusicContext : DbContext
    {
        public MusicContext(string connectionString)
            : base(connectionString)
        {
            Database.SetInitializer<MusicContext>(null);

        }

        public MusicContext()
            : this("MusicContext")
        {
            Database.SetInitializer<MusicContext>(null);
        }

        public virtual DbSet<Musician> Musicians { get; set; }

        public virtual DbSet<PlaylistService> PlaylistServices { get; set; }

        public virtual DbSet<Station> Stations { get; set; }
        public virtual DbSet<Track> Tracks { get; set; }
     //   public virtual DbSet<TrackStatus> TrackStatuses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<TrackStatus>()
            //    .HasMany(e => e.Tracks)
            //    .WithRequired(e => e.TrackStatus)
            //    .WillCascadeOnDelete(false);


            modelBuilder.Entity<PlaylistService>()
                .HasMany(e => e.Stations)
                .WithRequired(e => e.PlaylistService)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Musician>()
                .HasMany(e => e.Tracks)
                .WithRequired(e => e.Musician)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<Station>()
                .HasMany(e => e.Tracks)
                .WithMany(e => e.Stations)
                .Map(m =>
                {
                    m.MapLeftKey("StationId");
                    m.MapRightKey("TrackId");
                    m.ToTable("TracksStations");
                }
                    );

            modelBuilder.Entity<Track>()
                .HasMany(e => e.MixedNames)
                .WithRequired(e => e.Track)
                .WillCascadeOnDelete(true);

 
        }


    }
}