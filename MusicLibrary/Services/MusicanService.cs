using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using DataHelper.DBWork;
using DataHelper.Models;
using MusicLibrary.Data;
using MusicLibrary.Models;

namespace MusicLibrary.Services
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