using System.Collections.Generic;
using System.Linq;
using System.Data.Objects;
using System.Data.EntityClient;
using RDC.Shared.Framework.Data.EntityFramework;

namespace RDC.Shared.Framework.Data.EntityFramework
{
    public abstract class DataContextBaseEF : ObjectContext, IDataContextBaseEF
    {
        #region Properties
        #endregion

        #region Construction
        protected DataContextBaseEF(string connectionString, string containerName)
            : base(connectionString, containerName)
        {
            
        }

        protected DataContextBaseEF(EntityConnection connection, string containerName)
            : base(connection, containerName)
        {
            
        }
        #endregion

        #region Functionality
        public new void SaveChanges()
        {
            base.SaveChanges();
        }
        #endregion

        #region Events
        #endregion
    }
}
