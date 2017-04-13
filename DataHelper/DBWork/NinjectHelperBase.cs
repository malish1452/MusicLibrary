using System;
using System.Collections.Concurrent;
using System.Linq;
using Ninject;
using Ninject.Modules;

namespace DataHelper.DBWork
{
    public class NinjectHelperBase
    {
        private static readonly object Sync = new object();
        private static NinjectHelperBase _instance;
        private readonly IKernel _kernel;
        private static readonly ConcurrentDictionary<Type, IKernel> Kernels = new ConcurrentDictionary<Type, IKernel>();
        protected NinjectHelperBase(IKernel kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }

            _kernel = kernel;
        }

        public static IKernel Instance<TConfig>() where TConfig : NinjectConfigBase
        {
            if (Kernels.ContainsKey(typeof(TConfig)))
            {
                IKernel kernel;
                Kernels.TryGetValue(typeof(TConfig), out  kernel);
                if (kernel != null)
                {
                    return kernel;
                }
            }

            lock (Sync)
            {
                var args = Environment.GetCommandLineArgs();

                var isTestingMode =
                       args.Any(
                           arg => String.Equals(
                               arg,
                               "/testing",
                               StringComparison.CurrentCultureIgnoreCase));

                var configInstance = Activator.CreateInstance(typeof(TConfig)) as NinjectModule;

                var kernel = new StandardKernel(configInstance);
                _instance = new NinjectHelperBase(kernel);

                Kernels.AddOrUpdate(typeof(TConfig), _instance._kernel, (t, k) => _instance._kernel);

                return _instance._kernel;
            }
        }

        public static T Get<T>()
        {
            return Instance<NinjectConfigBase>().Get<T>();
        }
    }
}