using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CDWKS.Model.EF.Content;

namespace CDWKS.Repository.Content
{
    public interface IAutodeskFileTreeNodeRepository
    {
        AutodeskFileTreeNode GetByID(int id);
        IEnumerable<AutodeskFileTreeNode> GetByLibraryAndFamilyName(string owner, string library, string filename);
        IEnumerable<AutodeskFileTreeNode> GetAllByLibrary(string libary, string owner);
        void DeleteNode(AutodeskFileTreeNode node);
        void InsertNode(AutodeskFileTreeNode treeNode);
        void SaveGroupChanges();
    }

    public class AutodeskFileTreeNodeRepository : IAutodeskFileTreeNodeRepository
    {
        private readonly BXC_ContentModelEntities _context;

        public AutodeskFileTreeNodeRepository(BXC_ContentModelEntities context)
        {
            _context = context;
        }

        public AutodeskFileTreeNode GetByID(int id)
        {
            return _context.AutodeskFileTreeNodes.FirstOrDefault(n => n.Id == id);
        }

        public IEnumerable<AutodeskFileTreeNode> GetByLibraryAndFamilyName(string owner, string library, string filename)
        {
            Expression<Func<AutodeskFileTreeNode, bool>> expr =
                t => t.AutodeskFile.Name == filename && t.TreeNode.ContentLibrary.Name == library;
            return _context.AutodeskFileTreeNodes.Where(expr);
        }

        public IEnumerable<AutodeskFileTreeNode> GetAllByLibrary(string library, string owner)
        {
            Expression<Func<AutodeskFileTreeNode, bool>> expr =
                t => t.TreeNode.ContentLibrary.Name == library;
            return _context.AutodeskFileTreeNodes.Where(expr);
        }

        public void DeleteNode(AutodeskFileTreeNode node)
        {
            _context.DeleteObject(node);
        }

        public void InsertNode(AutodeskFileTreeNode treeNode)
        {
            _context.AddObject("AutodeskFileTreeNodes", treeNode);
            _context.SaveChanges();
        }

        public void SaveGroupChanges()
        {
            _context.SaveChanges();
        }
    }
}
