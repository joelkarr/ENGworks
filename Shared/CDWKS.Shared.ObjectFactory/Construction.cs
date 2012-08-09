using System.Collections.Generic;
using Ninject;
using Ninject.Modules;
using Ninject.Planning.Bindings;
namespace CDWKS.Shared.ObjectFactory
{


    public static class Construction
    {
        private static IKernel _kernel;

        /// <summary>
        /// Standard inversion-of-control kernel that is used for binding interfaces to objects and retrieving object based upon interfaces
        /// </summary>
        public static IKernel StandardKernel
        {
            get { return _kernel ?? (_kernel = new StandardKernel()); }
        }

   

        /// <summary>
        /// Avoid calling this function in non-unit-test code!  This function is strictly for unit testing
        /// </summary>
        public static void ResetStandardKernel()
        {
            _kernel = new StandardKernel();
        }

        /// <summary>
        /// Loads a supplied module with a collection of predefined bindings (production code)
        /// </summary>
        /// <param name="modules"></param>
        public static void LoadStandardModule(params INinjectModule[] modules)
        {
            StandardKernel.Load(modules);
        }

        /// <summary>
        /// Determines whether a given type has a resolvable binding
        /// </summary>
        public static bool HasBindingFor<T>(this IKernel kernel)
        {
            var bindings = StandardKernel.GetBindings(typeof(T));
            return bindings != null && ((IList<IBinding>) bindings).Count != 0;
        }
    }
}
