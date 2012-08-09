using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDC.Shared.Framework.BL.Core
{
    public static class BusinessObjectFactory
    {
        #region Properties
        #endregion

        #region Construction
        #endregion

        #region Functionality
        public static T CreateManager<T>() where T : BusinessManagerBase, new()
        {
            return BusinessManagerBase.Create<T>();
        }

        public static T CreateEntity<T>() where T : BusinessEntityBase, new()
        {
            return BusinessEntityBase.Create<T>();
        }

        public static T CreateEntityList<T>() where T : BusinessEntityListBase, new()
        {
            return BusinessEntityListBase.Create<T>();
        }


        #endregion

        #region Events
        #endregion
    }
}
