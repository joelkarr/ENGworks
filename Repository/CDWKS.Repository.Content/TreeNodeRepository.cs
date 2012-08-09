using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CDWKS.Model.EF.Content;
using log4net;

namespace CDWKS.Repository.Content
{
    public partial interface ITreeNodeRepository
    {
        TreeNode GetByID(int id);
        TreeNode GetByNameAndParent(string folder, string parentFolderName);
        TreeNode AddTreeNode(TreeNode newNode);
        TreeNode GetByNameAndParentId(string folder, int? parentId);
        List<TreeNode> GetAllByLibrary(string library, string owner);
        void DeleteNode(TreeNode treenode);
        void SaveGroupChanges();
    }

    public partial class TreeNodeRepository : ITreeNodeRepository
    {
        private readonly BXC_ContentModelEntities _context;
        private static readonly ILog Log = LogManager.GetLogger(typeof(ITreeNodeRepository));
        public TreeNodeRepository(BXC_ContentModelEntities context)
        {
            log4net.Config.XmlConfigurator.Configure();
            _context = context;
        }

        public TreeNode GetByID(int id)
        {
            return _context.TreeNodes.FirstOrDefault(t => t.Id == id);
        }

        public TreeNode GetByNameAndParent(string folder, string parentFolderName)
        {
            Expression<Func<TreeNode, bool>> expr = t => t.Name == folder && t.TreeNode1.Name == parentFolderName;
            return _context.TreeNodes.Where(expr).FirstOrDefault();
        }

        public TreeNode AddTreeNode(TreeNode newNode)
        {
            try
            {
                _context.AddToTreeNodes(newNode);
                _context.SaveChanges();
                return newNode;
            }
            catch (Exception ex)
            {
                Log.Error("CreateNode Failed", ex);
                return null;
            }

        }

        public TreeNode GetByNameAndParentId(string folder, int? parentId)
        {
            Expression<Func<TreeNode, bool>> expr = t => t.Name == folder && (parentId == null || t.TreeNode1.Id == parentId);
            return _context.TreeNodes.Where(expr).FirstOrDefault();
        }

        public List<TreeNode> GetAllByLibrary(string library, string owner)
        {
            Expression<Func<TreeNode, bool>> expr = t => t.ContentLibrary.Name == library;
            return _context.TreeNodes.Where(expr).ToList();
        }

        public void DeleteNode(TreeNode treenode)
        {
            _context.DeleteObject(treenode);

        }

        public void SaveGroupChanges()
        {
            _context.SaveChanges();
        }

    }
}
