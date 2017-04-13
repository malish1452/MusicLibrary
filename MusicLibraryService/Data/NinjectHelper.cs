using DataHelper.DBWork;
using Ninject;

namespace MusicLibraryService.Data
{
    public class NinjectHelper : NinjectHelperBase
    {
        protected NinjectHelper(IKernel kernel)
            : base(kernel)
        {
            // empty
        }

        public new static T Get<T>()
        {
            return Instance<NinjectConfig>().Get<T>();
        }
    } 
}