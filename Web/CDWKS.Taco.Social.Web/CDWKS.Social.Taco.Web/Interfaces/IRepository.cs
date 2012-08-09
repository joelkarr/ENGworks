using System;
using System.Linq;
using System.Linq.Expressions;

namespace CDWKS.Social.Taco.Interfaces
{
    /// <summary>
    /// Generic interface that can be used across repositories.
    /// </summary>
    public interface IRepository<E> where E : class   
    {
        void Add(E newEntity);
        void Remove(E entity);
        IQueryable<E> Find(Expression<Func<E, bool>> predicate);
        IQueryable<E> FindAll();
        void Update(E entity);
    }
}
