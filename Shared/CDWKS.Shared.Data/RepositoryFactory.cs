using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using RDC.Shared.Util.ObjectFactory;
using Ninject.Activation;
using RDC.Shared.Framework.Business;
using Ninject.Parameters;

namespace RDC.Shared.Framework.Data
{
    public static class RepositoryFactory<TBusinessEntity> 
        where TBusinessEntity : class, IBusinessEntity
    {
        #region Properties
        #endregion

        #region Construction
        /// <summary>
        /// Creates a new repository and passes data context into it from the outside
        /// </summary>
        /// <typeparam name="T">Interface implemented by the actual repository that needs to be created</typeparam>
        /// <param name="dataContext">Previously created Data Context</param>
        /// <returns></returns>
        public static T Create<T>(IDataContext dataContext) 
            where T : IRepositoryBase<TBusinessEntity>
        {
            return Construction.StandardKernel.Get<T>(new Parameter("dataContext", dataContext, true));
        }

        /// <summary>
        /// Creates a new repository.  This repository will create and dispose of its own data context
        /// </summary>
        /// <typeparam name="T">Interface implemented by the actual repository that needs to be created</typeparam>
        /// <returns></returns>
        public static T Create<T>()
            where T : IRepositoryBase<TBusinessEntity>
        {
            return Construction.StandardKernel.Get<T>(new Parameter("dataContext", null as object, true));
        }
        #endregion

        #region Functionality
        #endregion

        #region Events
        #endregion
    }

}
