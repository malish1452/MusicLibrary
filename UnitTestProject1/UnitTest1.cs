using System;
using System.Collections.Generic;
using System.Linq;
using DataHelper.DBWork;
using DataHelper.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicLibraryService.Data;
using MusicLibraryService.Services;


namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var db = NinjectHelper.Get<IDataProvider>();

            var rservice = new TrackOperationService(db);

            var namesAll = db.TrackRepository.GetAll().GroupBy(z => z.MixedName).Where(c=>c.Count()>1);

            var names = namesAll.Where(c => c.Count() > 1).OrderByDescending(z=>z.Count());

            foreach (var name in names)
            {
                string currentName = name.First().MixedName;

                var track = db.TrackRepository.FirstOrDefault(c => c.MixedName == currentName);

                foreach (
                    Track trackToDelete in
                        db.TrackRepository.Where(c => c.MixedName == track.MixedName && c.Id != track.Id))
                {
                    db.TrackRepository.Delete(trackToDelete);
                }
            }



            db.Save();
        }

        //var track = db.TrackRepository.GetById(18216);


        //var track =  

        //foreach (Track track1 in track)
        //{

        //    rservice.ParseSingleMixedName(track);

        //}


        // db.Save();


    }
}


