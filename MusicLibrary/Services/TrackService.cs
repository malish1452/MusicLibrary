using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using DataHelper.DBWork;
using DataHelper.Models;
using MusicLibrary.Data;

namespace MusicLibrary.Models
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