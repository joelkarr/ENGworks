using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using RDC.Shared.Framework.Data;
using RDC.Shared.Framework.Business;

namespace RDC.Shared.Framework.Data.EntityFramework
{

    public abstract class RepositoryBaseEF<TBusinessEntity> : RepositoryBase<TBusinessEntity>, IRepositoryBaseEF<TBusinessEntity>
        where TBusinessEntity : class, IBusinessEntity
    {
        #region Properties
        private ObjectContext DataContext
        {
            get
            {
                return _dataContext as ObjectContext;
            }
        }
        #endregion

        #region Construction
        protected RepositoryBaseEF(IDataContextBaseEF dataContext)
            : base(dataContext)
        {
        }

        protected RepositoryBaseEF() : base()
        {
        }
        #endregion

        #region Functionality

        public override TBusinessEntity Insert(TBusinessEntity entity)
        {
            DataContext.AddObject(DataContext.GetEntitySetName<TBusinessEntity>(), entity);
            if (IsDataContextLocal)
                PersistAll();
            return entity;
        }

        public void BulkInsert(
                                IEnumerable<TBusinessEntity> entities,
                                IList<string> omitColumns,
                                bool exposeNullableColumns = true,
                                bool flattenRelatedObjects = true,
                                int batchSize = 1000
        )
        {
            // We need to strip out the metadata and provider sections from the connection string
            const string filterRegex = @"(?:(?:metadata|provider)=[^;]*;|provider\ connection\ string=)";
            var regex = new Regex( filterRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase );
            string noMetaDataConn = regex.Replace( DataContext.Connection.ConnectionString, string.Empty );

            // Take the tick marks off the ends
            var tick = new char[] { '\'' };
            noMetaDataConn = noMetaDataConn.TrimStart( tick ).TrimEnd( tick );

            // Create the bulk copy object and write the data
            using( var bcp = new SqlBulkCopy( noMetaDataConn ) )
            {
                bcp.DestinationTableName = DataContext.GetEntitySetName<TBusinessEntity>();
                bcp.BatchSize = batchSize;
                using( var reader = entities.AsDataReader( exposeNullableColumns, flattenRelatedObjects, omitColumns ) )
                {
                    bcp.WriteToServer(reader);
                    //foreach (var entity in entities)
                    //{
                    //    DataContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Unchanged); // attach
                    //}
                }
            }
        }


        public override TBusinessEntity Update(TBusinessEntity entity)
        {
            AddOrAttach(entity);
            DataContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
            if (IsDataContextLocal)
                PersistAll();

            // clear the cache if any exists for this item
            var cacheKey =
                this.CacheKey(
                    DataContext.CreateEntityKey(DataContext.GetEntitySetName<TBusinessEntity>(), entity).
                        EntityKeyValues[0].Value.ToString());
            Cache.Remove(cacheKey);

            return entity;
        }

        public override void Delete(TBusinessEntity entity)
        {
            AddOrAttach(entity);
            DataContext.DeleteObject(entity);
            if (IsDataContextLocal)
                PersistAll();

            // clear the cache if any exists for this item
            var cacheKey =
                this.CacheKey(
                    DataContext.CreateEntityKey(DataContext.GetEntitySetName<TBusinessEntity>(), entity).
                        EntityKeyValues[0].Value.ToString());
            Cache.Remove(cacheKey);
        }

        protected void AddOrAttach(TBusinessEntity entity)
        {
            // Define an ObjectStateEntry and EntityKey for the current object.
            object originalItem;
            // Get the detached object's entity key.
            // Get the entity key of the updated object.

            var key = DataContext.CreateEntityKey(DataContext.GetEntitySetName<TBusinessEntity>(), entity);


            // Get the original item based on the entity key from the context or from the database -- database will be used only if context does not have the item
            // http://msdn.microsoft.com/en-us/library/bb738728.aspx
            if (DataContext.TryGetObjectByKey(key, out originalItem))
            {
                if (originalItem != entity) DataContext.ApplyCurrentValues<TBusinessEntity>(key.EntitySetName, entity);
            }
            else
                DataContext.AddObject(key.EntitySetName, entity);
        }
        #endregion

        #region Events
        #endregion
    }

}
