using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using RDC.Shared.Framework.Business;

namespace RDC.Shared.Framework.Data
{
    public interface ICriteria<TBusinessEntity>
        where TBusinessEntity : class, IBusinessEntity
    {
        ICriteria<TBusinessEntity> WithCacheKey(string cacheKey);

        ICriteria<TBusinessEntity> Concat(IEnumerable<TBusinessEntity> source);

        ICriteria<TBusinessEntity> DefaultIfEmpty();

        ICriteria<TBusinessEntity> DefaultIfEmpty(TBusinessEntity defaultValue);

        ICriteria<TBusinessEntity> Distinct();

        ICriteria<TBusinessEntity> Distinct(IEqualityComparer<TBusinessEntity> comparer);

        ICriteria<TBusinessEntity> Except(IEnumerable<TBusinessEntity> source);

        ICriteria<TBusinessEntity> Except(
            IEnumerable<TBusinessEntity> source, IEqualityComparer<TBusinessEntity> comparer);

        ICriteria<TBusinessEntity> Intersect(IEnumerable<TBusinessEntity> source);

        ICriteria<TBusinessEntity> Intersect(
            IEnumerable<TBusinessEntity> source, IEqualityComparer<TBusinessEntity> comparer);

        ICriteria<TBusinessEntity> OfType();

        ISortedCriteria<TBusinessEntity> OrderBy<TKey>(Expression<Func<TBusinessEntity, TKey>> keySelector);

        ISortedCriteria<TBusinessEntity> OrderBy<TKey>(
            Expression<Func<TBusinessEntity, TKey>> keySelector, IComparer<TKey> comparer);

        ISortedCriteria<TBusinessEntity> OrderByDescending<TKey>(Expression<Func<TBusinessEntity, TKey>> keySelector);

        ISortedCriteria<TBusinessEntity> OrderByDescending<TKey>(
            Expression<Func<TBusinessEntity, TKey>> keySelector, IComparer<TKey> comparer);

        ICriteria<TBusinessEntity> Reverse();

        ICriteria<TBusinessEntity> Skip(int count);

        ICriteria<TBusinessEntity> SkipWhile(Expression<Func<TBusinessEntity, bool>> predicate);

        ICriteria<TBusinessEntity> SkipWhile(Expression<Func<TBusinessEntity, int, bool>> predicate);

        ICriteria<TBusinessEntity> Take(int count);

        ICriteria<TBusinessEntity> TakeWhile(Expression<Func<TBusinessEntity, bool>> predicate);

        ICriteria<TBusinessEntity> TakeWhile(Expression<Func<TBusinessEntity, int, bool>> predicate);

        ICriteria<TBusinessEntity> Union(IEnumerable<TBusinessEntity> source);

        ICriteria<TBusinessEntity> Union(
            IEnumerable<TBusinessEntity> source, IEqualityComparer<TBusinessEntity> comparer);

        ICriteria<TBusinessEntity> Where(Expression<Func<TBusinessEntity, bool>> predicate);

        ICriteria<TBusinessEntity> Where(Expression<Func<TBusinessEntity, int, bool>> predicate);

        ICriteria<TBusinessEntity> Include(Expression<Func<TBusinessEntity, object>> includes);

    }
}