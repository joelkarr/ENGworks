using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CDWKS.Model.EF.Content;

namespace CDWKS.Repository.Content
{
    public interface IItemRepository
    {
        Item GetByID(int id);
        Item GetByNameAndAutodeskFileName(string family, string itemType, string owner);
        List<Item> GetByAutodeskFileId(int famId);
    }

    public class ItemRepository : IItemRepository
    {
        private readonly BXC_ContentModelEntities _context;

        public ItemRepository(BXC_ContentModelEntities context)
        {
            _context = context;
        }

        public Item GetByID(int id)
        {
            return _context.Items.FirstOrDefault(i => i.Id == id);
        }

        public Item GetByNameAndAutodeskFileName(string family, string itemType, string ownerId)
        {
            Expression<Func<Item, bool>> expr = item => item.AutodeskFile.Name == family 
                && item.AutodeskFile.MC_OwnerId == ownerId
                && item.Name == itemType;
            return _context.Items.Where(expr).FirstOrDefault();
        }

        public List<Item> GetByAutodeskFileId(int famId)
        {
            Expression<Func<Item, bool>> expr = item => item.AutodeskFile.Id == famId;
            return _context.Items.Where(expr).ToList();
        }
    }
}
