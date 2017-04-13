using System;
using System.Linq;
using System.Linq.Expressions;
using DataHelper.DBWork;
using DataHelper.Models;


namespace MusicLibraryService.Services
{  public interface ITrackService
        {
            IQueryable<Track> GetTracks();
            IQueryable<Track> GetTracksWhere(Expression<Func<Track, bool>> predicate);
            Track GetTrackById(int id);

        }
    public class TrackService:BaseService,ITrackService
    {
        public TrackService(IDataProvider db) : base(db)
        {
            
        }
        public IQueryable<Track> GetTracks()
        {
            return Db.TrackRepository.GetAll();
        }
        public IQueryable<Track> GetTracksWhere(Expression<Func<Track, bool>> predicate)
        {
            return Db.TrackRepository.Where(predicate);
        }
        public Track GetTrackById(int id)
        {
            return Db.TrackRepository.GetById(id);
        }


        
    }
}