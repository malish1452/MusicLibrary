using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MusicLibraryService.Services;

namespace MusicLibraryService.Logics
{
    public class PlaylistMonitor : IDisposable
    {
        private readonly IPlaylistOperationService _service;

        private Task _playlisMonitorTask;

        public PlaylistMonitor(IPlaylistOperationService service)
        {
            _service = service;
        }

        public void Dispose()
        {
            if (_playlisMonitorTask != null)
                _playlisMonitorTask.Dispose();
        }

        public Task RunLogic()
        {

            _playlisMonitorTask = Task.Delay(0)
                .ContinueWith(t => MonitorPlaylist());
            return _playlisMonitorTask;
        }

        private void MonitorPlaylist()
        {
            try
            {
             _service.ReloadLists();
            }
            catch 
            {
                
            }

            Task.Delay(new TimeSpan(0,1,0)).ContinueWith(t => MonitorPlaylist());
        }

    }
}
