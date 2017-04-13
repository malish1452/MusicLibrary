using System.Data.Entity;
using Ninject.Modules;

namespace DataHelper.DBWork
{
    public class NinjectConfigBase : NinjectModule
    {

        public NinjectConfigBase()
        {
            
        }
        public override void Load()
        {
            Bind<IDataProvider>()
                .To<EfDataProvider>();

            Bind<DbContext>().To<MusicContext>().WithConstructorArgument("MusicContext");

        }
    }
}