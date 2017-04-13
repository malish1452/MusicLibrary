using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataHelper.DBWork;
using DataHelper.Models;


namespace MusicLibrary.Controllers
{
    public class ValuesController : ApiController
    {
     
        // GET api/values
        public ICollection<Track> Get()
        {
           DBHelper.TestConnection(ConfigurationManager.ConnectionStrings["MusicContext"].ConnectionString);
          
            return new Collection<Track>
            {
                new Track {Id = 1, MixedName = "Тест-тест"},
                new Track {Id = 2, Path = "\\12.0.0.1\test"}

            };
        }

        // GET api/values/5
        public string Get(int id)
        {
            

            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}