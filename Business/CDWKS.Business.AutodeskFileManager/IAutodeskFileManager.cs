using System;
using System.Collections.Generic;
using CDWKS.Model.EF.BIMXchange;


namespace CDWKS.Business.AutodeskFileManager
{
    public interface IAutodeskFileManager :IDisposable
    {

        string CreateDownloadPackage(List<Int32> itemId);

        #region Indexer
        bool AddOrUpdateAutodeskFile(AutodeskFile file, bool overwrite, BXCModelEntities context);
        bool AddTypeToFile(Item item, string fileName, string owner, BXCModelEntities context);
        IEnumerable<AutodeskFileTreeNode> GetAllTreeNodesForLibrary(string libraryName, string ownerId, BXCModelEntities context);
        TreeNode GetTreeNodeForLibary(string folder, int? parentId, int libraryId, BXCModelEntities context);
        bool RemoveAllAutodeskFileTreeNodesForLibrary(string library, string owner, BXCModelEntities context);
        bool RemoveAllTreeNodesForLibrary(string library, string owner, BXCModelEntities context);
        AutodeskFile GetAutodeskFile(string fileName, string libraryName, BXCModelEntities context);
        void RemoveAutodeskFileTreeNode(AutodeskFileTreeNode treeNode, BXCModelEntities context);
        void AddAutodeskFileTreeNode(AutodeskFileTreeNode treeNode, BXCModelEntities context);
        Library GetContentLibrary(string libraryName, string ownerId, BXCModelEntities context);
        Library AddContentLibrary(string libraryName, BXCModelEntities context);
        TreeNode AddTreeNode(TreeNode getTreeNodeFromPoco, BXCModelEntities context);
        TreeNode GetTreeNode(int value, BXCModelEntities context);
        AutodeskFile GetAutodeskFileById(int family, BXCModelEntities context);
        #endregion
    }

}
