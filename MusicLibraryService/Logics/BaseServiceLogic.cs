using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataHelper.DBWork;
using MusicLibraryService.Data;


namespace MusicLibraryService.Logics
{
    public class BaseServiceLogic:IDisposable
    {
        protected IDataProvider Db;
        private PlaylistMonitor _playlistMonitor;
        private StationLoader _stationLoader;
        private MixedNamesParser _mixedNamesParser;

        public void Dispose()
        {
            _playlistMonitor.Dispose();
            _stationLoader.Dispose();
            _mixedNamesParser.Dispose();
        }

        public void StartProcess()
        {
            //_playlistMonitor = NinjectHelper.Get<PlaylistMonitor>();
            //_playlistMonitor.RunLogic().Wait();

            //_stationLoader = NinjectHelper.Get<StationLoader>();
            //_stationLoader.RunLogic().Wait();

            _mixedNamesParser = NinjectHelper.Get<MixedNamesParser>();
            _mixedNamesParser.RunLogic().Wait();




        }
    }
}
