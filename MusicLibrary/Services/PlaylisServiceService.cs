using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using DataHelper.DBWork;
using DataHelper.Models;

namespace MusicLibrary.Services
{

    public interface IPlaylistServiceService
    {
        IQueryable<PlaylistService> GetPlaylistServices();
        IQueryable<PlaylistService> GetPlaylistServicesWhere(Expression<Func<PlaylistService, bool>> predicate);
        void ReloadLists();
        void ReloadPlradio();
        PlaylistService GetPlaylistServiceById(int id);
    }
    public class PlaylistServiceService:BaseService,IPlaylistServiceService
    {
        public PlaylistServiceService(IDataProvider db) : base(db)
        {
            
        }

        public IQueryable<PlaylistService> GetPlaylistServices()
        {
            return Db.PlaylistServiceRepository.GetAll();
        }

        public IQueryable<PlaylistService> GetPlaylistServicesWhere(Expression<Func<PlaylistService, bool>> predicate)
        {
            return Db.PlaylistServiceRepository.Where(predicate);
        }

        public PlaylistService GetPlaylistServiceById(int id)
        {
            return Db.PlaylistServiceRepository.GetById(id);
        }

        public void ReloadLists()
        {
            //
        }

        public void ReloadPlradio()
        {
            //
        }
    }
}