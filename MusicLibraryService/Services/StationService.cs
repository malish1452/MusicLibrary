using System;
using System.Linq;
using System.Linq.Expressions;
using DataHelper.DBWork;
using DataHelper.Models;

namespace MusicLibraryService.Services
{
    public interface IStationService
    {
        IQueryable<Station> GetStations();
        IQueryable<Station> GetStationsWhere(Expression<Func<Station, bool>> predicate);

        Station GetStationById(int id);
    }


    public class StationService : BaseService, IStationService
    {
        public StationService(IDataProvider db) : base(db)
        {
            
        }

        public IQueryable<Station> GetStations()
        {
            return Db.StationRepository.GetAll();
        }

        public IQueryable<Station> GetStationsWhere(Expression<Func<Station, bool>> predicate)
        {
            return Db.StationRepository.Where(predicate);
        }

        public Station GetStationById(int id)
        {
            return Db.StationRepository.GetById(id);
        }
    }
    
    
}