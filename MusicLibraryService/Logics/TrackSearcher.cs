using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicLibraryService.Services;

namespace MusicLibraryService.Logics
{
    public class TrackSearcher:IDisposable
    {
        private readonly ITrackOperationService _service;

        private Task _trackSearcherTask;

        public TrackSearcher(ITrackOperationService service)
        {
            _service = service;
        }

        public void Dispose()
        {
            if (_trackSearcherTask!=null)
                _trackSearcherTask.Dispose();
        }

        public void Start()
        {
            _trackSearcherTask = Task.Delay(0).ContinueWith(t => SearchTracks());
        }
        public void  RunLogic()
        {
            _trackSearcherTask = Task.Delay(0).ContinueWith(t => SearchTracks());
           //return _trackSearcherTask;
        }

        private void SearchTracks()
        {
            _service.SearchTracks();

            Task.Delay(new TimeSpan(0, 0, 5)).ContinueWith(t => SearchTracks()).Wait();
        }
    }
}
