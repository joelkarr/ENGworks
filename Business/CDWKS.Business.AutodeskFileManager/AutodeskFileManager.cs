using System;
using System.Collections.Generic;
using CDWKS.Model.EF.Content;
using CDWKS.Repository.Content;
using log4net;

namespace CDWKS.Business.AutodeskFileManager
{
    public class AutodeskFileManager : IAutodeskFileManager
    {
        #region Constructors
        private static readonly ILog Log = LogManager.GetLogger(typeof(IAutodeskFileManager));
        public static IAutodeskFileManager CreateForIOC()
        {
            log4net.Config.XmlConfigurator.Configure();
            return new AutodeskFileManager();
        }

        #endregion

        #region Methods


        public bool AddTypeToFile(Item item, string fileName, string owner, BXC_ContentModelEntities context = null)
        {
            context = context ?? new BXC_ContentModelEntities();
                var fileRepo = new AutodeskFileRepository(context);
                var existingFile = fileRepo.GetByNameAndOwner(fileName, owner);
                if (existingFile != null)
                {
                    var itemRepo = new ItemRepository(context);
                    var existingItem = itemRepo.GetByNameAndAutodeskFileName(fileName, item.Name, owner);
                    if (existingItem == null)
                    {
                        fileRepo.AddItem(item, existingFile);
                        return true;
                    }
                    Log.Error(string.Format("Item Not Added:{0} {1}", item.Name, fileName));
                    //Should never occur - Deleting all Items on File update.
                }
                return false;
            
        }

        public IEnumerable<AutodeskFileTreeNode> GetAllTreeNodesForLibrary(string libraryName, string ownerId, BXC_ContentModelEntities context = null)
        {
            context = context ?? new BXC_ContentModelEntities();
                var fileNodeRepo = new AutodeskFileTreeNodeRepository(context);
                return fileNodeRepo.GetAllByLibrary(libraryName, ownerId);
            
        }

        public TreeNode GetTreeNodeForLibary(string folder, int? parentId, int libraryId, BXC_ContentModelEntities context = null)
        {
            context = context ?? new BXC_ContentModelEntities();
                var nodeRepo = new TreeNodeRepository(context);
                return nodeRepo.GetByNameAndParentId(folder, parentId)
                       ?? CreateNewTreeNode(folder, parentId, libraryId, context);
            
        }

        private TreeNode CreateNewTreeNode(string folder, int? parentId, int libraryId, BXC_ContentModelEntities context)
        {
                var nodeRepo = new TreeNodeRepository(context);
                var libraryRepo = new ContentLibraryRepository(context);
                var newNode = new TreeNode
                                  {
                                      Name = folder,
                                      TreeNode1 =
                                          parentId == null
                                              ? null
                                              : nodeRepo.GetByID(parentId.Value),
                                      ContentLibrary = libraryRepo.GetByID(libraryId)
                                  };
                nodeRepo.AddTreeNode(newNode);
                return newNode;
            

        }

        public AutodeskFile GetAutodeskFile(string fileName, string ownerName, BXC_ContentModelEntities context = null)
        {
            context = context ?? new BXC_ContentModelEntities();
                var fileRepo = new AutodeskFileRepository(context);
                return fileRepo.GetByNameAndOwner(fileName, ownerName);
            
        }

        public void RemoveAutodeskFileTreeNode(AutodeskFileTreeNode treeNode, BXC_ContentModelEntities context = null)
        {
            context = context ?? new BXC_ContentModelEntities();
                var fileNodeRepo = new AutodeskFileTreeNodeRepository(context);
                fileNodeRepo.DeleteNode(treeNode);
            

            
        }

        public void AddAutodeskFileTreeNode(AutodeskFileTreeNode treeNode, BXC_ContentModelEntities context = null)
        {
            try
            {

            context = context ?? new BXC_ContentModelEntities();
                var fileNodeRepo = new AutodeskFileTreeNodeRepository(context);
                fileNodeRepo.InsertNode(treeNode);
            }
                                    

            catch (Exception ex)
            {
                Log.Error("CreateNode Failed", ex);
               
            }
        }

        public ContentLibrary GetContentLibrary(string libraryName, string ownerId, BXC_ContentModelEntities context = null)
        {
            context = context ?? new BXC_ContentModelEntities();
                var libraryRepo = new ContentLibraryRepository(context);
                return libraryRepo.GetByNameAndOwner(libraryName, ownerId);
            

        }

        public ContentLibrary AddContentLibrary(string libraryName, BXC_ContentModelEntities context = null)
        {
            context = context ?? new BXC_ContentModelEntities();
                var libraryRepo = new ContentLibraryRepository(context);
                return libraryRepo.AddContentLibrary(libraryName);
            
        }

        public bool RemoveNodes(IEnumerable<AutodeskFileTreeNode> Nodes, BXC_ContentModelEntities context = null)
        {
            context = context ?? new BXC_ContentModelEntities();

                var fileNodeRepo = new AutodeskFileTreeNodeRepository(context);

                foreach (var node in Nodes)
                {
                    fileNodeRepo.DeleteNode(node);
                }
                fileNodeRepo.SaveGroupChanges();
                return true;
            
        }

        public bool RemoveAllAutodeskFileTreeNodesForLibrary(string library, string owner, BXC_ContentModelEntities context = null)
        {
            context = context ?? new BXC_ContentModelEntities();
                var fileNodeRepo = new AutodeskFileTreeNodeRepository(context);
                var aftreenodes = fileNodeRepo.GetAllByLibrary(library, owner);
                foreach (var aftreenode in aftreenodes)
                {
                    fileNodeRepo.DeleteNode(aftreenode);
                }
                fileNodeRepo.SaveGroupChanges();
                return true;
            
        }

        public bool RemoveAllTreeNodesForLibrary(string library, string owner, BXC_ContentModelEntities context = null)
        {
            context = context ?? new BXC_ContentModelEntities();

            var nodeRepo = new TreeNodeRepository(context);
            var treenodes = nodeRepo.GetAllByLibrary(library, owner);
            foreach (var treenode in treenodes)
            {
                nodeRepo.DeleteNode(treenode);
            }
            nodeRepo.SaveGroupChanges();
            return true;

        }

        public bool AddOrUpdateAutodeskFile(AutodeskFile file, bool overwrite, BXC_ContentModelEntities context = null)
        {
            context = context ?? new BXC_ContentModelEntities();
            var fileRepo = new AutodeskFileRepository(context);
            var existingFile = fileRepo.GetByNameAndOwner(file.Name, file.MC_OwnerId);
            if (existingFile == null)
            {
                fileRepo.InsertFile(file);
            }
            else
            {
                existingFile.TypeCatalogHeader = file.TypeCatalogHeader;
                fileRepo.UpdateFile(existingFile);
            }
            return true;

        }

        #endregion

        public TreeNode AddTreeNode(TreeNode nextNode, BXC_ContentModelEntities context = null)
        {
            context = context ?? new BXC_ContentModelEntities();
            try
            {


                var nodeRepo = new TreeNodeRepository(context);
              
                return nodeRepo.AddTreeNode(nextNode);
                                }

            catch (Exception ex)
            {
                Log.Error("CreateNode Failed", ex);
                return null;
            }

        }

        public TreeNode GetTreeNode(int value, BXC_ContentModelEntities context = null)
        {
            context = context ?? new BXC_ContentModelEntities();
            var nodeRepo = new TreeNodeRepository(context);
            return nodeRepo.GetByID(value);

        }

        public AutodeskFile GetAutodeskFileById(int family, BXC_ContentModelEntities context = null)
        {
            context = context ?? new BXC_ContentModelEntities();
                var fileRepo = new AutodeskFileRepository(context);
                return fileRepo.GetByID(family);
        }

        public void Dispose()
        {
        }
    }
}
