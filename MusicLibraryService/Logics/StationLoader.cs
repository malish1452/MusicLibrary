using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicLibraryService.Services;

namespace MusicLibraryService.Logics
{
    internal class StationLoader : IDisposable
    {
        private readonly IStationOperationService _service;

        private Task _statioLoaderTask;

        public StationLoader(IStationOperationService service)
        {
            _service = service;
        }

        public void Dispose()
        {
            if (_statioLoaderTask != null)
                _statioLoaderTask.Dispose();
        }

        public Task RunLogic()
        {

            _statioLoaderTask = Task.Delay(0)
                .ContinueWith(t => LoadStations());
            return _statioLoaderTask;
        }

        private void LoadStations()
        {
            try
            {
                _service.ProcessStations();
            }
            catch
            {

            }
            Task.Delay(new TimeSpan(0, 1, 0)).ContinueWith(t => LoadStations());
        }
    }
}

