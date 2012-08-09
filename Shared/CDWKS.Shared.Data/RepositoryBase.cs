using System;
using RDC.Shared.Framework.Business;
using RDC.Shared.Util.Caching;
using RDC.Shared.Util.Common;
using RDC.Shared.Util.ObjectFactory;
using Ninject;

namespace RDC.Shared.Framework.Data
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Base class for all repository objects in the RDC domain
    /// </summary>
    /// <typeparam name="TBusinessEntity"></typeparam>
    public abstract class RepositoryBase<TBusinessEntity> : IRepositoryBase<TBusinessEntity>
        where TBusinessEntity : class, IBusinessEntity 
    {
        #region Properties
        // Track whether Dispose has been called.
        private bool _disposed;
        private readonly bool _isDataContextLocal = true;
        protected IDataContext _dataContext;

        /// <summary>
        /// Tracks whether the data context was passed into this repository from outside or created internally
        /// If the data context was passed from outside, repository should not be calling its own SaveChanges() or disposing of the data context
        /// If the data context was created internally, repository's Update/Insert/Delete methods will call SaveChanges() at the end and dispose data context on Dispose()
        /// </summary>
        protected bool IsDataContextLocal
        {
            get
            {
                return this._isDataContextLocal;
            }
        }


        private readonly bool _isCacheEnabled;
        protected bool IsCacheEnabled
        {
            get
            {
                return _isCacheEnabled;
            }
        }

        private readonly bool _isCacheForCollectionsEnabled;
        protected bool IsCacheForCollectionsEnabled
        {
            get
            {
                return _isCacheForCollectionsEnabled;
            }
        }

        private ICache<TBusinessEntity> _cache;

        /// <summary>
        /// Repository-specific Cache provider
        /// </summary>
        protected virtual ICache<TBusinessEntity> Cache
        {
            get
            {
                return _cache
                       ??
                       (_cache =
                        IsCacheEnabled
                            ? Construction.StandardKernel.Get<ICache<TBusinessEntity>>()
                            : new MissingCacheProvider<TBusinessEntity>());
            }
        }

        private ICache<IEnumerable<TBusinessEntity>> _cacheForCollections;

        /// <summary>
        /// Repository-specific Cache provider for enumerable objects
        /// </summary>
        protected virtual ICache<IEnumerable<TBusinessEntity>> CacheForCollections
        {
            get
            {
                return _cacheForCollections
                       ??
                       (_cacheForCollections =
                        IsCacheEnabled
                            ? Construction.StandardKernel.Get<ICache<IEnumerable<TBusinessEntity>>>()
                            : new MissingCacheProvider<IEnumerable<TBusinessEntity>>());
            }
        }

        protected virtual string CacheKeyPrefix
        {
            get
            {
                return string.Format("{0}_Key", typeof(TBusinessEntity).Name);
            }
        }

        protected virtual string CacheKey(string id)
        {
            return string.Format("{0}_{1}", CacheKeyPrefix, id);
        }

        #endregion

        #region Construction

        /// <summary>
        /// Base constructor - must be called at all times in order for Caching to work
        /// </summary>
        protected RepositoryBase()
        {
            _isCacheEnabled = Construction.StandardKernel.HasBindingFor<ICache<TBusinessEntity>>();
            _isCacheForCollectionsEnabled =
                Construction.StandardKernel.HasBindingFor<ICache<IEnumerable<TBusinessEntity>>>();
        }

        protected RepositoryBase(IDataContext dataContext)
            : this()
        {
            if (dataContext == null) throw new RepositoryOperationException("Repository Data Context cannot be empty.");
            _dataContext = dataContext;
            _isDataContextLocal = false;
        }

        #endregion

        #region Functionality

        /// <summary>
        /// Generic function that is able to access internal Query from any Criteria object
        /// </summary>
        /// <typeparam name="TEntity">IBusinessEntity based class</typeparam>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected virtual IQueryable<TEntity> GetQueryFromCriteria<TEntity>(ICriteria<TEntity> criteria)
            where TEntity : class, IBusinessEntity
        {
            var internalCriteria = (criteria as IInternalCriteria<TEntity>);
            if (internalCriteria == null)
                throw new ApplicationException("Invalid criteria object passed to repository");
            return internalCriteria.Query;
        }

        /// <summary>
        /// Generic function that is able to create any criteria object based upon a custom IQueryable
        /// </summary>
        /// <typeparam name="TEntity">IBusinessEntity based class</typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual ICriteria<TEntity> CreateCriteria<TEntity>(IQueryable<TEntity> query)
            where TEntity : class, IBusinessEntity
        {
            return new Criteria<TEntity>(query);
        }

        /// <summary>
        /// Returns a collection of entities that matches passed criteria.  If no criteria was passed, returns every entity in the data store of matching type
        /// </summary>
        /// <param name="criteria">Optional criteria object.  Use CreateCriteria function to generate a new Criteria object</param>
        /// <returns>List of entities that match passed criteria</returns>
        public virtual IEnumerable<TBusinessEntity> GetListByCriteria(ICriteria<TBusinessEntity> criteria = null)
        {
            if (criteria == null) return Query.AsEnumerable();

            var internalCriteria = criteria as IInternalCriteria<TBusinessEntity>;

            if (internalCriteria == null) throw new ApplicationException("Invalid criteria object passed to repository");

            var result = default(IEnumerable<TBusinessEntity>);
            if (IsCacheForCollectionsEnabled && !string.IsNullOrEmpty(internalCriteria.CacheKey)) 
                result = CacheForCollections.Get(internalCriteria.CacheKey);

            if (result == null)
            {
                result = internalCriteria.Query.AsEnumerable();

                if (IsCacheForCollectionsEnabled && !string.IsNullOrEmpty(internalCriteria.CacheKey) && result != null) 
                    this.CacheForCollections.Store(internalCriteria.CacheKey, result);
            }

            return result;
        }

        /// <summary>
        /// Returns an entity that matches passed criteria.  If no criteria was passed, returns first entity in the data store of matching type
        /// </summary>
        /// <param name="criteria">Optional criteria object.  Use CreateCriteria function to generate a new Criteria object</param>
        /// <returns>List of entities that match passed criteria</returns>
        public virtual TBusinessEntity GetFirstByCriteria(ICriteria<TBusinessEntity> criteria = null)
        {

            if (criteria == null) return Query.FirstOrDefault();

            var internalCriteria = criteria as IInternalCriteria<TBusinessEntity>;

            if (internalCriteria == null) throw new ApplicationException("Invalid criteria object passed to repository");

            var result = default(TBusinessEntity);
            if (IsCacheEnabled && !string.IsNullOrEmpty(internalCriteria.CacheKey)) result = Cache.Get(internalCriteria.CacheKey);

            if (result == null)
            {
                result = internalCriteria.Query.FirstOrDefault();

                if (IsCacheEnabled && result != null && !string.IsNullOrEmpty(internalCriteria.CacheKey)) Cache.Store(internalCriteria.CacheKey, result);
            }
            return result;
        }

        /// <summary>
        /// Create default criteria object that this repository can evaluate
        /// </summary>
        /// <returns></returns>
        public ICriteria<TBusinessEntity> CreateDefaultCriteria()
        {
            return CreateCriteria(Query);
        }

        /// <summary>
        /// Provides access to Repository's core IQueryable interface for Criteria object to consume internally
        /// Must be overriden by individual repository
        /// </summary>
        protected abstract IQueryable<TBusinessEntity> Query { get; }

        /// <summary>
        /// Updates a completely populated entity to the data context
        /// If repository was not injected with an external data context, SaveChanges() is called after data context was populated
        /// </summary>
        /// <param name="entity">Fully populated business entity</param>
        /// <returns></returns>
        public abstract TBusinessEntity Update(TBusinessEntity entity);


        /// <summary>
        /// Inserts a completely populated entity to the data context
        /// If repository was not injected with an external data context, SaveChanges() is called after data context was populated
        /// </summary>
        /// <param name="entity">Fully populated business entity</param>
        /// <returns></returns>
        public abstract TBusinessEntity Insert(TBusinessEntity entity);

        /// <summary>
        /// Deletes an entity from the data context
        /// If repository was not injected with an external data context, SaveChanges() is called
        /// </summary>
        /// <param name="entity">Fully populated business entity</param>
        /// <returns></returns>
        public abstract void Delete(TBusinessEntity entity);

        /// <summary>
        /// Abstracts away call to SaveChanges()
        /// If data context was provided from the outside, SaveChanges() is not allowed to be called
        /// </summary>
        protected virtual void PersistAll()
        {
            //Only allow PersistAll() to be called when dataContext was NOT passed to the repository from the outside
            if (IsDataContextLocal)
                _dataContext.SaveChanges();
            else
                throw new RepositoryOperationException("Repository cannot SaveChanges when Data Context is passed from outside the reposiory");
        }
        #endregion

        #region Events
        #endregion

        #region IDisposable

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // only Dispose of the _dataContext is the repository itself has created it.  IsDataContextLocal in such cases is true
                    if (this._dataContext != null && this.IsDataContextLocal)
                    {
                        // Dispose managed resources.
                        this._dataContext.Dispose();
                        this._dataContext = null;
                    }
                }

                // Note disposing has been done.
                this._disposed = true;
            }
        }
        
        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~RepositoryBase()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
