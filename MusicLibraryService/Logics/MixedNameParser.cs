using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicLibraryService.Services;

namespace MusicLibraryService.Logics
{
    public class MixedNamesParser : IDisposable
    {
        private readonly ITrackOperationService _service;

        private Task _mixedNameParserTask;

        public MixedNamesParser(ITrackOperationService service)
        {
            _service = service;
        }

        public void Dispose()
        {
            if (_mixedNameParserTask != null)
                _mixedNameParserTask.Dispose();
        }

        public Task RunLogic()
        {

            _mixedNameParserTask = Task.Delay(0).ContinueWith(t => ParseMixedNames());
          return _mixedNameParserTask;
        }

        private void ParseMixedNames()
        {
            try
            {   
                _service.ParseNewMixedNames();
            }
            catch
            {

            }
            Task.Delay(new TimeSpan(0,1,0)).ContinueWith(t => ParseMixedNames()).Wait();
        }
    }
}
