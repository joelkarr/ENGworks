using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using RDC.Shared.Framework.Business;

namespace RDC.Shared.Framework.Data
{

    public interface ISortedCriteria<TBusinessEntity> : ICriteria<TBusinessEntity>
        where TBusinessEntity : class, IBusinessEntity
    {
        ISortedCriteria<TBusinessEntity> ThenBy<TKey>(Expression<Func<TBusinessEntity, TKey>> keySelector);

        ISortedCriteria<TBusinessEntity> ThenBy<TKey>(
            Expression<Func<TBusinessEntity, TKey>> keySelector, IComparer<TKey> comparer);

        ISortedCriteria<TBusinessEntity> ThenByDescending<TKey>(Expression<Func<TBusinessEntity, TKey>> keySelector);

        ISortedCriteria<TBusinessEntity> ThenByDescending<TKey>(
            Expression<Func<TBusinessEntity, TKey>> keySelector, IComparer<TKey> comparer);

    }
}
