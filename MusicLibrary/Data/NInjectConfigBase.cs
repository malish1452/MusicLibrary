using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DataHelper.DBWork;
using Ninject.Modules;
using MusicLibrary.Data;

namespace MusicLibrary.Data
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

            Bind<DbContext>()
    .To<MusicContext>()
    .WithConstructorArgument("MusicContext");
        }
    }
}