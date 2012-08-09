using System;
using System.Linq;
using System.Linq.Expressions;
using CDWKS.Model.EF.Content;

namespace CDWKS.Repository.Content
{
    public interface IAutodeskFileRepository
    {
        AutodeskFile GetByID(int id);
        AutodeskFile GetByNameAndOwner(string name, string ownername);
        void InsertFile(AutodeskFile file);
        void UpdateFile(AutodeskFile file);
        void DeleteFile(AutodeskFile file);
        void AddItem(Item item, AutodeskFile file);
        void DeleteItem(Item item);
        void UpdateItem(Item item);
        void RemoveAllItems(AutodeskFile file);
    }

    public class AutodeskFileRepository : IAutodeskFileRepository
    {
        private readonly BXC_ContentModelEntities _context;

        public AutodeskFileRepository(BXC_ContentModelEntities context)
        {
            _context = context;
        }

        public AutodeskFile GetByID(int id)
        {
            return (from f in _context.AutodeskFiles where f.Id == id select f).FirstOrDefault();
        }

        public AutodeskFile GetByNameAndOwner(string name, string mcOwnerId)
        {
            Expression<Func<AutodeskFile, bool>> expr = a => a.Name == name & a.MC_OwnerId == mcOwnerId;
            return _context.AutodeskFiles.Where(expr).FirstOrDefault();
        }

        public void InsertFile(AutodeskFile file)
        {
            Expression<Func<AutodeskFile, bool>> expr = a => a.Name == file.Name && a.MC_OwnerId == file.MC_OwnerId;
            var existingFile = _context.AutodeskFiles.Where(expr).FirstOrDefault();
            if (existingFile == null)
            {
                _context.AddObject("AutodeskFiles", file);
                _context.SaveChanges();
            }

        }

        public void CreateTreeNodes(AutodeskFile file, string treePath)
        {
            //add tree nodes
            var nodes = treePath.Split('/');
            var libraryName = nodes[0];
            var root = GetOrCreateTreeNode(null, libraryName, libraryName);
            var fileTreeNode = new AutodeskFileTreeNode {AutodeskFile = file, TreeNode = root};
            _context.AddObject("AutodeskFileTreeNodes", fileTreeNode);
            _context.SaveChanges();
            var parent = root;

            for (var i = 1; i < nodes.Count(); i++)
            {
                var treeNode = GetOrCreateTreeNode(parent, nodes[i], libraryName);
                fileTreeNode = new AutodeskFileTreeNode {AutodeskFile = file, TreeNode = parent};
                _context.AddObject("AutodeskFileTreeNodes", fileTreeNode);
                _context.SaveChanges();
                parent = treeNode;
            }
        }

        public void DeleteFile(AutodeskFile file)
        {
            //delete all item parameters
            foreach (var item in file.Items)
            {
                foreach (var param in item.Parameters)
                {
                    _context.DeleteObject(param);
                }
                _context.SaveChanges();
                _context.DeleteObject(item);
                _context.SaveChanges();
            }
            foreach (var node in file.AutodeskFileTreeNodes)
            {
                _context.DeleteObject(node);
            }

            _context.DeleteObject(file);
            _context.SaveChanges();
        }


        public void AddItem(Item item, AutodeskFile file)
        {
            var newItem = new Item
                              {
                                  Name = item.Name,
                                  TypeCatalogEntry = item.TypeCatalogEntry,
                                  AutodeskFile = file
                              };
            _context.AddObject("Items", newItem);
            _context.SaveChanges();
            foreach (var param in item.Parameters)
            {
                GetParameter(param.SearchName.Name, param.SearchValue.Value, newItem.Id);
            }

            _context.SaveChanges();
        }

        public void DeleteItem(Item item)
        {
            _context.DeleteObject(item);
            _context.SaveChanges();
        }

        public void UpdateItem(Item item)
        {
            Expression<Func<Item, bool>> expr = a => a.Id == item.Id;
            var existingItem = _context.Items.Where(expr).FirstOrDefault();

            if (existingItem != null)
            {
                existingItem.Name = item.Name;
                existingItem.TypeCatalogEntry = item.TypeCatalogEntry;
                foreach (var param in existingItem.Parameters)
                {
                    _context.DeleteObject(param);
                }
                _context.SaveChanges();
                foreach (var param in item.Parameters)
                {

                    GetParameter(param.SearchName.Name, param.SearchValue.Value, existingItem.Id);
                    _context.SaveChanges();
                }
            }
            _context.SaveChanges();
        }

        public void RemoveAllItems(AutodeskFile file)
        {
            Expression<Func<Item, bool>> expr = i => i.AutodeskFile.Id == file.Id;
            var items = _context.Items.Where(expr).ToList();
            foreach (var item in items)
            {
                Expression<Func<Parameter, bool>> paramExpr = p => p.Item.Id == item.Id;
                var parameters =
                    _context.Parameters.Where(paramExpr);
                foreach (var p in parameters)
                {
                    _context.DeleteObject(p);

                }
                _context.DeleteObject(item);
                _context.SaveChanges();
            }
        }

        public void UpdateFile(AutodeskFile file)
        {
            RemoveAllItems(file);
            _context.SaveChanges();

        }

        private TreeNode GetOrCreateTreeNode(TreeNode parent, string name, string libraryName)
        {

            Expression<Func<TreeNode, bool>> expr =
                a => a.Name == name && (parent == null || a.TreeNode1.Id == parent.Id);

            var existingNode = _context.TreeNodes.Where(expr).FirstOrDefault();
            if (existingNode != null)
            {
                return existingNode;
            }
            var node = new TreeNode
                           {Name = name, TreeNode1 = {Id = parent.Id}, ContentLibrary = GetContentLibrary(libraryName)};
            _context.TreeNodes.AddObject(node);
            _context.SaveChanges();
            return node;
        }

        private ContentLibrary GetContentLibrary(string libraryName)
        {
            Expression<Func<ContentLibrary, bool>> expr = a => a.Name == libraryName;
            return _context.ContentLibraries.Where(expr).FirstOrDefault();
        }


        private void GetParameter(string searchName, string searchValue, int itemId)
        {
            Expression<Func<Parameter, bool>> expr = a => a.Item.Id == itemId
                                                          && a.SearchName.Name == searchName &&
                                                          a.SearchValue.Value == searchValue;
            var existingParam = _context.Parameters.Where(expr).FirstOrDefault();
            if (existingParam != null)
            {
                return;
            }
            var searchNameObject = GetSearchName(searchName);
            var searchValueObject = GetSearchValue(searchValue);
            var newParam = new Parameter
                               {SearchName = searchNameObject, SearchValue = searchValueObject, Item = GetItem(itemId)};
            _context.AddObject("Parameters", newParam);
            _context.SaveChanges();
        }

        private Item GetItem(int itemId)
        {
            Expression<Func<Item, bool>> expr = i => i.Id == itemId;
            return _context.Items.Where(expr).FirstOrDefault();
        }

        private SearchName GetSearchName(string searchName)
        {
            Expression<Func<SearchName, bool>> expr = a => a.Name == searchName;
            var existingName = _context.SearchNames.Where(expr).FirstOrDefault();
            if (existingName != null)
            {
                return existingName;
            }
            var name = new SearchName {Name = searchName};
            _context.AddObject("SearchNames", name);
            _context.SaveChanges();
            return name;
        }

        private SearchValue GetSearchValue(string searchValue)
        {
            _context.SaveChanges();
            Expression<Func<SearchValue, bool>> expr = a => a.Value == searchValue;
            var existingvalue = _context.SearchValues.Where(expr).FirstOrDefault();
            if (existingvalue != null)
            {
                return existingvalue;
            }
            var value = new SearchValue {Value = searchValue};
            _context.AddObject("SearchValues", value);
            _context.SaveChanges();
            return value;
        }
    }
}
