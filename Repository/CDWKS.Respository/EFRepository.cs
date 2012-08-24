using System;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;

namespace CDWKS.Respository.Account
{
    public class EFRepository<E> : IRepository<E> where E : class
    {
        #region Members

        private readonly ObjectSet<E> _objectSet;
        private readonly ObjectContext _context;

        #endregion

        #region Constructor(s)

        public EFRepository(ObjectContext context)
        {
            if (context != null)
            {
                _context = context;
                _objectSet = context.CreateObjectSet<E>();
            }
            else
            {
                throw new ArgumentNullException("context");
            }
        }

        #endregion

        #region Methods

        public void Add(E newEntity)
        {
            _objectSet.AddObject(newEntity);
        }

        public void Remove(E entity)
        {
            _objectSet.DeleteObject(entity);
        }

        public IQueryable<E> Find(Expression<Func<E, bool>> predicate)
        {
            return _objectSet.Where(predicate);
        }

        public IQueryable<E> FindAll()
        {
            return _objectSet;
        }

        public void Update(E entity)
        {
            _context.ApplyCurrentValues(_objectSet.EntitySet.Name, entity);
        }

        #endregion
    }
}
