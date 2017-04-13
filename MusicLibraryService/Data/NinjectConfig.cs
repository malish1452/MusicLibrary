
using DataHelper.DBWork;
using MusicLibraryService.Services;

namespace MusicLibraryService.Data
{
 public class NinjectConfig : NinjectConfigBase
        {
            public NinjectConfig(): base()
            {
                // empty
            }
            public override void Load()
            {
                base.Load();

                Bind<IPlaylistOperationService>().To<PlaylistOperationService>();
                Bind<IStationOperationService>().To<StationOperationService>();
                Bind<ITrackOperationService>().To<TrackOperationService>();


            }
        }
    }

