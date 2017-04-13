using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using DataHelper.DBWork;
using DataHelper.Models;
using MusicLibrary.Data;
using MusicLibrary.Models;

namespace MusicLibrary.Controllers
{
    public class PlaylistServiceController : ApiController
    {
        //
        // GET: /PlaylistService/
       private IDataProvider db = NinjectHelper.Get<IDataProvider>();

        public string get()
        {
            return "I'm alive";
        }
        public void Post(PlaylistService service)
        {
            db.PlaylistServiceRepository.Add(service);
            //return String.Format("Получено задание на создание сервиса {0}", service.Name);
            db.Save();
        }


    }
}
