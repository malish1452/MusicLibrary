using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;

namespace MusicLibrary.Data
{
    public class NinjectHelper : NinjectHelperBase
    {
        protected NinjectHelper(IKernel kernel)
            : base(kernel)
        {
            // empty
        }

        //public new static T Get<T>()
        //{
        //    return Instance<NinjectConfig>().Get<T>();
        //}
    } 
}