using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDC.Shared.Framework.Business;
using System.Linq.Expressions;

namespace RDC.Shared.Framework.Data
{
    using System.Data.Objects;

    internal class Criteria<TBusinessEntity> : ICriteria<TBusinessEntity>, ISortedCriteria<TBusinessEntity>, IInternalCriteria<TBusinessEntity>
        where TBusinessEntity : class, IBusinessEntity
    {
        #region Properties

        IQueryable<TBusinessEntity> IInternalCriteria<TBusinessEntity>.Query { get; set; }

        string IInternalCriteria<TBusinessEntity>.CacheKey { get; set; }

        private IQueryable<TBusinessEntity> QueryProxy
        {
            get
            {
                return (this as IInternalCriteria<TBusinessEntity>).Query;
            }
            set
            {
                (this as IInternalCriteria<TBusinessEntity>).Query = value;
            }

        }

        #endregion

        #region Construction
        internal Criteria(IQueryable<TBusinessEntity> query)
        {
            QueryProxy = query;
        }

        #endregion

        #region Functionality

        public ICriteria<TBusinessEntity> WithCacheKey(string cacheKey)
        {
            (this as IInternalCriteria<TBusinessEntity>).CacheKey = cacheKey;
            return this;
        }

        public ICriteria<TBusinessEntity> Concat(IEnumerable<TBusinessEntity> source)
        {
            QueryProxy = QueryProxy.Concat(source);
            return this;
        }

        public ICriteria<TBusinessEntity> DefaultIfEmpty()
        {
            QueryProxy = QueryProxy.DefaultIfEmpty();
            return this;
        }

        public ICriteria<TBusinessEntity> DefaultIfEmpty(TBusinessEntity defaultValue)
        {
            QueryProxy = QueryProxy.DefaultIfEmpty(defaultValue);
            return this;
        }

        public ICriteria<TBusinessEntity> Distinct()
        {
            QueryProxy = QueryProxy.Distinct();
            return this;
        }

        public ICriteria<TBusinessEntity> Distinct(IEqualityComparer<TBusinessEntity> comparer)
        {
            QueryProxy = QueryProxy.Distinct(comparer);
            return this;
        }

        public ICriteria<TBusinessEntity> Except(IEnumerable<TBusinessEntity> source)
        {
            QueryProxy = QueryProxy.Except(source);
            return this;
        }

        public ICriteria<TBusinessEntity> Except(IEnumerable<TBusinessEntity> source, IEqualityComparer<TBusinessEntity> comparer)
        {
            QueryProxy = QueryProxy.Except(source, comparer);
            return this;
        }

        public ICriteria<TBusinessEntity> Intersect(IEnumerable<TBusinessEntity> source)
        {
            QueryProxy = QueryProxy.Intersect(source);
            return this;
        }

        public ICriteria<TBusinessEntity> Intersect(IEnumerable<TBusinessEntity> source, IEqualityComparer<TBusinessEntity> comparer)
        {
            QueryProxy = QueryProxy.Intersect(source, comparer);
            return this;
        }

        public ICriteria<TBusinessEntity> Reverse()
        {
            QueryProxy = QueryProxy.Reverse();
            return this;
        }

        public ICriteria<TBusinessEntity> Skip(int count)
        {
            QueryProxy = QueryProxy.Skip(count);
            return this;
        }

        public ICriteria<TBusinessEntity> SkipWhile(Expression<Func<TBusinessEntity, bool>> predicate)
        {
            QueryProxy = QueryProxy.SkipWhile(predicate);
            return this;
        }

        public ICriteria<TBusinessEntity> SkipWhile(Expression<Func<TBusinessEntity, int, bool>> predicate)
        {
            QueryProxy = QueryProxy.SkipWhile(predicate);
            return this;
        }

        public ICriteria<TBusinessEntity> Take(int count)
        {
            QueryProxy = QueryProxy.Take(count);
            return this;
        }

        public ICriteria<TBusinessEntity> TakeWhile(Expression<Func<TBusinessEntity, bool>> predicate)
        {
            QueryProxy = QueryProxy.TakeWhile(predicate);
            return this;
        }

        public ICriteria<TBusinessEntity> TakeWhile(Expression<Func<TBusinessEntity, int, bool>> predicate)
        {
            QueryProxy = QueryProxy.TakeWhile(predicate);
            return this;
        }

        public ICriteria<TBusinessEntity> Union(IEnumerable<TBusinessEntity> source)
        {
            QueryProxy = QueryProxy.Union(source);
            return this;
        }

        public ICriteria<TBusinessEntity> Union(IEnumerable<TBusinessEntity> source, IEqualityComparer<TBusinessEntity> comparer)
        {
            QueryProxy = QueryProxy.Union(source, comparer);
            return this;
        }

        public ICriteria<TBusinessEntity> Where(Expression<Func<TBusinessEntity, bool>> predicate)
        {
            QueryProxy = QueryProxy.Where(predicate);
            return this;
        }

        public ICriteria<TBusinessEntity> Where(Expression<Func<TBusinessEntity, int, bool>> predicate)
        {
            QueryProxy = QueryProxy.Where(predicate);
            return this;
        }

        public ICriteria<TBusinessEntity> OfType()
        {
            QueryProxy = QueryProxy.OfType<TBusinessEntity>();
            return this;
        }

        public ISortedCriteria<TBusinessEntity> OrderBy<TKey>(Expression<Func<TBusinessEntity, TKey>> keySelector)
        {
            QueryProxy = QueryProxy.OrderBy(keySelector);
            return this;
        }

        public ISortedCriteria<TBusinessEntity> OrderBy<TKey>(Expression<Func<TBusinessEntity, TKey>> keySelector, IComparer<TKey> comparer)
        {
            QueryProxy = QueryProxy.OrderBy(keySelector, comparer);
            return this;
        }

        public ISortedCriteria<TBusinessEntity> OrderByDescending<TKey>(Expression<Func<TBusinessEntity, TKey>> keySelector)
        {
            QueryProxy = QueryProxy.OrderByDescending(keySelector);
            return this;
        }

        public ISortedCriteria<TBusinessEntity> OrderByDescending<TKey>(Expression<Func<TBusinessEntity, TKey>> keySelector, IComparer<TKey> comparer)
        {
            QueryProxy = QueryProxy.OrderByDescending(keySelector, comparer);
            return this;
        }

        public ISortedCriteria<TBusinessEntity> ThenBy<TKey>(Expression<Func<TBusinessEntity, TKey>> keySelector)
        {
            QueryProxy = (QueryProxy as IOrderedQueryable<TBusinessEntity>).ThenBy(keySelector);
            return this;
        }

        public ISortedCriteria<TBusinessEntity> ThenBy<TKey>(Expression<Func<TBusinessEntity, TKey>> keySelector, IComparer<TKey> comparer)
        {
            QueryProxy = (QueryProxy as IOrderedQueryable<TBusinessEntity>).ThenBy(keySelector, comparer);
            return this;
        }

        public ISortedCriteria<TBusinessEntity> ThenByDescending<TKey>(Expression<Func<TBusinessEntity, TKey>> keySelector)
        {
            QueryProxy = (QueryProxy as IOrderedQueryable<TBusinessEntity>).ThenByDescending(keySelector);
            return this;
        }

        public ISortedCriteria<TBusinessEntity> ThenByDescending<TKey>(Expression<Func<TBusinessEntity, TKey>> keySelector, IComparer<TKey> comparer)
        {
            QueryProxy = (QueryProxy as IOrderedQueryable<TBusinessEntity>).ThenByDescending(keySelector, comparer);
            return this;
        }

        public ICriteria<TBusinessEntity> Include(Expression<Func<TBusinessEntity, object>> includes)
        {
            if (QueryProxy is ObjectQuery<TBusinessEntity>)
            {
                var q = (QueryProxy as ObjectQuery<TBusinessEntity>);
                QueryProxy = q.Include(includes);
            }
            else
            {
                throw new ApplicationException("Include statement is not supported on this Criteria");
            }

            return this;
        }
        #endregion
    }
}
