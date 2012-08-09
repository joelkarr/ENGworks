using System;
using System.Collections.Generic;
using CDWKS.Model.EF.Content;

namespace CDWKS.Business.AutodeskFileManager
{
    public interface IAutodeskFileManager :IDisposable
    {
        bool AddOrUpdateAutodeskFile(AutodeskFile file, bool overwrite, BXC_ContentModelEntities context);
        bool AddTypeToFile(Item item, string fileName, string owner, BXC_ContentModelEntities context);
        IEnumerable<AutodeskFileTreeNode> GetAllTreeNodesForLibrary(string libraryName, string ownerId, BXC_ContentModelEntities context);
        TreeNode GetTreeNodeForLibary(string folder, int? parentId, int libraryId, BXC_ContentModelEntities context);
        bool RemoveAllAutodeskFileTreeNodesForLibrary(string library, string owner, BXC_ContentModelEntities context);
        bool RemoveAllTreeNodesForLibrary(string library, string owner, BXC_ContentModelEntities context);
        AutodeskFile GetAutodeskFile(string fileName, string libraryName, BXC_ContentModelEntities context);
        void RemoveAutodeskFileTreeNode(AutodeskFileTreeNode treeNode, BXC_ContentModelEntities context);
        void AddAutodeskFileTreeNode(AutodeskFileTreeNode treeNode, BXC_ContentModelEntities context);
        ContentLibrary GetContentLibrary(string libraryName, string ownerId, BXC_ContentModelEntities context);
        ContentLibrary AddContentLibrary(string libraryName, BXC_ContentModelEntities context);
        TreeNode AddTreeNode(TreeNode getTreeNodeFromPoco, BXC_ContentModelEntities context);
        TreeNode GetTreeNode(int value, BXC_ContentModelEntities context);
        AutodeskFile GetAutodeskFileById(int family, BXC_ContentModelEntities context);
    }

}
