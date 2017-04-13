using System;
using System.Linq;
using System.Linq.Expressions;
using DataHelper.DBWork;
using DataHelper.Models;

namespace MusicLibraryService.Services
{       
    public interface IMusicianService
        {
            IQueryable<Musician> GetMusicians();
            IQueryable<Musician> GetMusiciansWhere(Expression<Func<Musician, bool>> predicate);

            Musician GetMusicianById(int id);
        }
    public class MusicanService:BaseService,IMusicianService

    {
        public MusicanService(IDataProvider db) : base(db)
        {
            
        }

        public IQueryable<Musician> GetMusicians()
        {
            return Db.MusicianRepository.GetAll();
        }

        public IQueryable<Musician> GetMusiciansWhere(Expression<Func<Musician, bool>> predicate)
        {
            return Db.MusicianRepository.Where(predicate);
        }

        public Musician GetMusicianById(int id)
        {
            return Db.MusicianRepository.GetById(id);
        }
     

    }
}