using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDC.Shared.Framework.Data;
using RDC.Shared.Framework.Business;

namespace RDC.Shared.Framework.Data.EntityFramework
{
    public interface IRepositoryBaseEF<TBusinessEntity> : IRepositoryBase<TBusinessEntity>
        where TBusinessEntity : class, IBusinessEntity
    {
        void BulkInsert(
                        IEnumerable<TBusinessEntity> entities,
                        IList<string> omitColumns,
                        bool exposeNullableColumns = true,
                        bool flattenRelatedObjects = true,
                        int batchSize = 1000 );
    }
}
