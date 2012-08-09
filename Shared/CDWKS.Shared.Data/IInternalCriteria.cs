using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDC.Shared.Framework.Business;

namespace RDC.Shared.Framework.Data
{

    internal interface IInternalCriteria<TBusinessEntity> : ICriteria<TBusinessEntity>, ISortedCriteria<TBusinessEntity>
        where TBusinessEntity : class, IBusinessEntity
    {
        IQueryable<TBusinessEntity> Query { get; set; }

        string CacheKey { get; set; }

    }
}
