using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDC.Shared.Framework.Business;

namespace RDC.Shared.Framework.Data
{
    public interface IRepositoryBase<TBusinessEntity> : IDisposable
        where TBusinessEntity : class, IBusinessEntity
    {
        /// <summary>
        /// Get a default Criteria object that can be populated by consumers of this repository and used in GetFirstByCriteria and GetListByCriteria functions
        /// </summary>
        /// <returns></returns>
        ICriteria<TBusinessEntity> CreateDefaultCriteria();

        /// <summary>
        /// Returns first entity that matches passed criteria.  If no criteria was passed, returns first entity in the data store of matching type
        /// </summary>
        /// <param name="criteria">Optional criteria object.  Use one of CreateCriteria functions to generate a new Criteria object</param>
        /// <returns>List of entities that match passed criteria</returns>
        TBusinessEntity GetFirstByCriteria(ICriteria<TBusinessEntity> criteria = null);

        /// <summary>
        /// Returns a collection of entities that matches passed criteria.  If no criteria was passed, returns every entity in the data store of matching type
        /// </summary>
        /// <param name="criteria">Optional criteria object.  Use one of CreateCriteria functions to generate a new Criteria object</param>
        /// <returns>List of entities that match passed criteria</returns>
        IEnumerable<TBusinessEntity> GetListByCriteria(ICriteria<TBusinessEntity> criteria = null);

        /// <summary>
        /// Updates a completely populated entity to the data context
        /// If repository was not injected with an external data context, SaveChanges() is called after data context was populated
        /// </summary>
        /// <param name="entity">Fully populated business entity</param>
        /// <returns></returns>
        TBusinessEntity Update(TBusinessEntity entity);

        /// <summary>
        /// Inserts a completely populated entity to the data context
        /// If repository was not injected with an external data context, SaveChanges() is called after data context was populated
        /// </summary>
        /// <param name="entity">Fully populated business entity</param>
        /// <returns></returns>
        TBusinessEntity Insert(TBusinessEntity entity);

        /// <summary>
        /// Deletes an entity from the data context
        /// If repository was not injected with an external data context, SaveChanges() is called
        /// </summary>
        /// <param name="entity">Fully populated business entity</param>
        /// <returns></returns>
        void Delete(TBusinessEntity entity);
    }
}
