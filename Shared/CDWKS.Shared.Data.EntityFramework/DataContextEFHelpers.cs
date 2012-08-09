using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Metadata.Edm;
using System.Data.Objects;

namespace RDC.Shared.Framework.Data.EntityFramework
{
    public static class DataContextEFHelpers
    {
        public static EntityType GetCSpaceEntityType<TBusinessEntity>(this MetadataWorkspace workspace)
        {
            // Make sure the assembly for "T" is loaded 
            workspace.LoadFromAssembly(typeof(TBusinessEntity).Assembly);
            // Try to get the ospace type and if that is found 
            // look for the cspace type too. 
            EntityType ospaceEntityType = null;
            if (workspace.TryGetItem<EntityType>(
                typeof(TBusinessEntity).FullName, DataSpace.OSpace, out ospaceEntityType))
            {
                StructuralType cspaceEntityType = null;
                if (workspace.TryGetEdmSpaceType(ospaceEntityType, out cspaceEntityType))
                {
                    return cspaceEntityType as EntityType;
                }
            }
            return null;
        }

        public static IEnumerable<EntityType> GetHierarchy(this EntityType entityType)
        {
            while (entityType != null)
            {
                yield return entityType;
                entityType = entityType.BaseType as EntityType;
            }
        }

        public static IEnumerable<EntitySet> GetEntitySets(this MetadataWorkspace workspace, EntityType type)
        {
            return from current in type.GetHierarchy()
                   from container in workspace.GetItems<EntityContainer>(DataSpace.CSpace)
                   from set in (container.BaseEntitySets.OfType<EntitySet>()).Where(e => e.ElementType == current)
                   select set;
        }

        public static EntitySet GetEntitySet<TBusinessEntity>(this ObjectContext ctx)
        {
            if (ctx == null) throw new ArgumentNullException("ctx");
            var wkspace = ctx.MetadataWorkspace;
            return wkspace.GetEntitySets(wkspace.GetCSpaceEntityType<TBusinessEntity>()).Single();
        }

        public static string GetEntitySetName<TBusinessEntity>(this ObjectContext ctx)
        {
            return GetEntitySet<TBusinessEntity>(ctx).Name;
        }

        public static void AttachToDefaultSet<TBusinessEntity>(this ObjectContext ctx, TBusinessEntity entity)
        {
            ctx.AttachTo(ctx.GetEntitySetName<TBusinessEntity>(), entity);
        }

        public static void AddToDefaultSet<TBusinessEntity>(this ObjectContext ctx, TBusinessEntity entity)
        {
            ctx.AddObject(ctx.GetEntitySetName<TBusinessEntity>(), entity);
        }
    }
}
