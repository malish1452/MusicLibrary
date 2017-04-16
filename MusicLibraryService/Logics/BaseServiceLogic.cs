using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataHelper.DBWork;
using MusicLibraryService.Data;
using System.Configuration;

namespace MusicLibraryService.Logics
{
    public class BaseServiceLogic:IDisposable
    {
        protected IDataProvider Db;
        private PlaylistMonitor _playlistMonitor;
        private StationLoader _stationLoader;
        private MixedNamesParser _mixedNamesParser;
        private TrackSearcher _trackSearcher;

        public void Dispose()
        {
            _playlistMonitor.Dispose();
            _stationLoader.Dispose();
            _mixedNamesParser.Dispose();
        }

        public void StartProcess()
        {
            _playlistMonitor = NinjectHelper.Get<PlaylistMonitor>();
            _playlistMonitor.RunLogic();

            _stationLoader = NinjectHelper.Get<StationLoader>();
            _stationLoader.RunLogic();

           _mixedNamesParser = NinjectHelper.Get<MixedNamesParser>();
           _mixedNamesParser.RunLogic();

            
            _trackSearcher = NinjectHelper.Get<TrackSearcher>();

            _trackSearcher.RunLogic();




        }
        
    }
}
