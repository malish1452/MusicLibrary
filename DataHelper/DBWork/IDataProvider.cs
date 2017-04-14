using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
//using System.Data.Objects;
using System.Linq;
using DataHelper.Models;
using System.Data.Entity.Core.Objects;

namespace DataHelper.DBWork
{

    public interface IDataProvider : IDisposable
    {
        IGenericRepository<Track> TrackRepository { get; }
        IGenericRepository<Station> StationRepository { get; }
        IGenericRepository<Musician> MusicianRepository { get; }
        IGenericRepository<PlaylistService> PlaylistServiceRepository { get; }
        IGenericRepository<TrackStatus> TrackStatusRepository { get; }

        IGenericRepository<MixedNames> MixedNamesRepository { get; } 

        void Save();
    }

    public class EfDataProvider : IDataProvider 
    {

        private IGenericRepository<Track> _trackRepository;
        private IGenericRepository<Musician> _musicianRepository;
        private IGenericRepository<PlaylistService> _playListServiceRepository;
        private IGenericRepository<Station> _stationRepository;
        private IGenericRepository<TrackStatus> _trackStatusRepository;
        private IGenericRepository<MixedNames> _mixedNamesRepository;





        public EfDataProvider(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IGenericRepository<Track> TrackRepository
        {
            get { return _trackRepository ?? (_trackRepository = new EfGenericRepository<Track>(DbContext)); }
        }

        public IGenericRepository<Musician> MusicianRepository
        {
            get { return _musicianRepository ?? (_musicianRepository = new EfGenericRepository<Musician>(DbContext)); }
        }

        public IGenericRepository<PlaylistService> PlaylistServiceRepository
        {
            get
            {
                return _playListServiceRepository ??
                       (_playListServiceRepository = new EfGenericRepository<PlaylistService>(DbContext));
            }
        }

        public IGenericRepository<TrackStatus> TrackStatusRepository
        {
            get
            {
                return _trackStatusRepository ??
                       (_trackStatusRepository = new EfGenericRepository<TrackStatus>(DbContext));
            }
        }

        public IGenericRepository<Station> StationRepository
        {
            get { return _stationRepository ?? (_stationRepository = new EfGenericRepository<Station>(DbContext)); }
        }

        public IGenericRepository<MixedNames> MixedNamesRepository
        {
            get
            {
                return _mixedNamesRepository ?? (_mixedNamesRepository = new EfGenericRepository<MixedNames>(DbContext));
            }
        } 

        protected DbContext DbContext;

        public void Save()
        {
            try
            {
                int i = DbContext.SaveChanges();
            }
                //catch (DbEntityValidationException dbEx)
                //{
                //    foreach (var validationErrors in dbEx.EntityValidationErrors)
                //    {
                //        foreach (var validationError in validationErrors.ValidationErrors)
                //        {
                //            Trace.TraceInformation("Property: {0} Error: {1}",
                //                                    validationError.PropertyName,
                //                                    validationError.ErrorMessage);
                //        }
                //    }
                //}
            catch (Exception e)
            {
                try
                {
                    var objectContext = ((IObjectContextAdapter) DbContext).ObjectContext;
                        // перечитывает контекст , тем самым откатывает измененя 
                    var refreshableObjects = DbContext.ChangeTracker.Entries().Select(c => c.Entity).ToList();
                    objectContext.Refresh(RefreshMode.StoreWins, refreshableObjects);
                }
                catch (Exception ex)
                {
                    // we just trying to refresh context on exception, ignoring refresh errors if arise

                }

                throw new DbUpdateException("Exception while saving db context.", e);
            }
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}