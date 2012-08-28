using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using CDWKS.Model.EF.BIMXchange;
using CDWKS.Repository.Content;
using CDWKS.Shared.ObjectFactory;
using Ionic.Zip;
using Ninject;
using log4net;

namespace CDWKS.Business.AutodeskFileManager
{
    public class AutodeskFileManager : IAutodeskFileManager
    {

        #region Properties
        private IItemRepository _itemRepository { get; set; }

        public IItemRepository ItemRepository
        {
            get { return _itemRepository = _itemRepository ?? Construction.StandardKernel.Get<IItemRepository>(); }
        }

        private IAutodeskFileRepository _autodeskFileRepository { get; set; }

        public IAutodeskFileRepository AutodeskFileRepository
        {
            get { return _autodeskFileRepository = _autodeskFileRepository ?? Construction.StandardKernel.Get<IAutodeskFileRepository>(); }
        }
        #endregion

        #region Constructors
        private static readonly ILog Log = LogManager.GetLogger(typeof(IAutodeskFileManager));
        public static IAutodeskFileManager CreateForIOC()
        {
            log4net.Config.XmlConfigurator.Configure();
            return new AutodeskFileManager();
        }

        #endregion

        #region Download
        public string CreateDownloadPackage(List<Int32> itemIds)
        {
            var guid = Guid.NewGuid().ToString();
            try
            {
                var zipFilePath = String.Format("{0}\\{1}\\",
                                                   ConfigurationManager.AppSettings["DownloadDirectory"],
                                                   guid);
                var zipFileName = String.Format("{0}{1}",
                                                   zipFilePath,
                                                   "package.zip");
                //Get Disctinct Families
                var items = itemIds.Select(id => ItemRepository.GetByID(id)).ToList();
                var distinctFamilies = (from f in items select f.AutodeskFile.Id).Distinct();
                var famIds = distinctFamilies as List<int> ?? distinctFamilies.ToList();
                var count = famIds.Count();
                if (count == 0)
                    return "Not found in ENGworks Library";
                foreach (var famId in famIds)
                {
                    var family = AutodeskFileRepository.GetByID(famId);

                    if (family != null)
                    {
                        var filePath = String.Format("{0}\\{1}.rfa",
                                                     ConfigurationManager.AppSettings["ContentDirectory"],
                                                     family.Name);
                        AddtoZip(filePath, family.Name, zipFileName, zipFilePath);
                    }
                    if (family != null && !String.IsNullOrWhiteSpace(family.TypeCatalogHeader))
                    {
                        var typeCatalogPath = String.Format("{0}\\{1}\\{2}",
                                                               ConfigurationManager.AppSettings["DownloadDirectory"],
                                                               guid,
                                                               family.Name + ".txt");

                        var sw = File.CreateText(typeCatalogPath);
                        {
                            sw.WriteLine(family.TypeCatalogHeader);
                            var typeCatalogs = ItemRepository.GetByAutodeskFileId(famId);
                            foreach (var tc in typeCatalogs)
                            {
                                sw.WriteLine(tc.TypeCatalogEntry);
                            }
                        }

                        sw.Close();
                        AddtoZip(typeCatalogPath, family.Name + ".txt", zipFileName, zipFilePath);

                    }
                }
                return guid;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void AddtoZip(string filePath, string fileName, string zipFileName, string zipFilePath)
        {
            // Create users session directory
            if (!Directory.Exists(zipFilePath))
                Directory.CreateDirectory(zipFilePath);

            if (!File.Exists(zipFileName))
            {
                var zip = new ZipFile();
                zip.Save(zipFileName);
            }
            using (var zip = ZipFile.Read(zipFileName))
            {
                if (File.Exists(filePath))
                {
                    var selection = from e in zip.Entries
                                    where e.FileName == fileName
                                    select e;
                    if (selection.Count() == 0)
                    {
                        zip.AddFile(filePath, String.Empty);
                    }
                }
                zip.Save();
            }
        }
  
        #endregion

        #region Indexer
        public bool AddTypeToFile(Item item, string fileName, string owner, BXCModelEntities context = null)
        {
            context = context ?? new BXCModelEntities();
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

        public IEnumerable<AutodeskFileTreeNode> GetAllTreeNodesForLibrary(string libraryName, string ownerId, BXCModelEntities context = null)
        {
            context = context ?? new BXCModelEntities();
                var fileNodeRepo = new AutodeskFileTreeNodeRepository(context);
                return fileNodeRepo.GetAllByLibrary(libraryName, ownerId);
            
        }

        public TreeNode GetTreeNodeForLibary(string folder, int? parentId, int libraryId, BXCModelEntities context = null)
        {
            context = context ?? new BXCModelEntities();
                var nodeRepo = new TreeNodeRepository(context);
                return nodeRepo.GetByNameAndParentId(folder, parentId)
                       ?? CreateNewTreeNode(folder, parentId, libraryId, context);
            
        }

        private TreeNode CreateNewTreeNode(string folder, int? parentId, int libraryId, BXCModelEntities context)
        {
                var nodeRepo = new TreeNodeRepository(context);
                var libraryRepo = new LibraryRepository(context);
                var newNode = new TreeNode
                                  {
                                      Name = folder,
                                      TreeNode1 =
                                          parentId == null
                                              ? null
                                              : nodeRepo.GetByID(parentId.Value),
                                      Library = libraryRepo.GetByID(libraryId)
                                  };
                nodeRepo.AddTreeNode(newNode);
                return newNode;
            

        }

        public AutodeskFile GetAutodeskFile(string fileName, string ownerName, BXCModelEntities context = null)
        {
            context = context ?? new BXCModelEntities();
                var fileRepo = new AutodeskFileRepository(context);
                return fileRepo.GetByNameAndOwner(fileName, ownerName);
            
        }

        public void RemoveAutodeskFileTreeNode(AutodeskFileTreeNode treeNode, BXCModelEntities context = null)
        {
            context = context ?? new BXCModelEntities();
                var fileNodeRepo = new AutodeskFileTreeNodeRepository(context);
                fileNodeRepo.DeleteNode(treeNode);
            

            
        }

        public void AddAutodeskFileTreeNode(AutodeskFileTreeNode treeNode, BXCModelEntities context = null)
        {
            try
            {

            context = context ?? new BXCModelEntities();
                var fileNodeRepo = new AutodeskFileTreeNodeRepository(context);
                fileNodeRepo.InsertNode(treeNode);
            }
                                    

            catch (Exception ex)
            {
                Log.Error("CreateNode Failed", ex);
               
            }
        }

        public Library GetContentLibrary(string libraryName, string ownerId, BXCModelEntities context = null)
        {
            context = context ?? new BXCModelEntities();
                var libraryRepo = new LibraryRepository(context);
                return libraryRepo.GetByNameAndOwner(libraryName, ownerId);
            

        }

        public Library AddContentLibrary(string libraryName, BXCModelEntities context = null)
        {
            context = context ?? new BXCModelEntities();
                var libraryRepo = new LibraryRepository(context);
                return libraryRepo.AddLibrary(libraryName);
            
        }

        public bool RemoveNodes(IEnumerable<AutodeskFileTreeNode> Nodes, BXCModelEntities context = null)
        {
            context = context ?? new BXCModelEntities();

                var fileNodeRepo = new AutodeskFileTreeNodeRepository(context);

                foreach (var node in Nodes)
                {
                    fileNodeRepo.DeleteNode(node);
                }
                fileNodeRepo.SaveGroupChanges();
                return true;
            
        }

        public bool RemoveAllAutodeskFileTreeNodesForLibrary(string library, string owner, BXCModelEntities context = null)
        {
            context = context ?? new BXCModelEntities();
                var fileNodeRepo = new AutodeskFileTreeNodeRepository(context);
                var aftreenodes = fileNodeRepo.GetAllByLibrary(library, owner);
                foreach (var aftreenode in aftreenodes)
                {
                    fileNodeRepo.DeleteNode(aftreenode);
                }
                fileNodeRepo.SaveGroupChanges();
                return true;
            
        }

        public bool RemoveAllTreeNodesForLibrary(string library, string owner, BXCModelEntities context = null)
        {
            context = context ?? new BXCModelEntities();

            var nodeRepo = new TreeNodeRepository(context);
            var treenodes = nodeRepo.GetAllByLibrary(library, owner);
            foreach (var treenode in treenodes)
            {
                nodeRepo.DeleteNode(treenode);
            }
            nodeRepo.SaveGroupChanges();
            return true;

        }

        public bool AddOrUpdateAutodeskFile(AutodeskFile file, bool overwrite, BXCModelEntities context = null)
        {
            context = context ?? new BXCModelEntities();
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

        public TreeNode AddTreeNode(TreeNode nextNode, BXCModelEntities context = null)
        {
            context = context ?? new BXCModelEntities();
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

        public TreeNode GetTreeNode(int value, BXCModelEntities context = null)
        {
            context = context ?? new BXCModelEntities();
            var nodeRepo = new TreeNodeRepository(context);
            return nodeRepo.GetByID(value);

        }

        public AutodeskFile GetAutodeskFileById(int family, BXCModelEntities context = null)
        {
            context = context ?? new BXCModelEntities();
                var fileRepo = new AutodeskFileRepository(context);
                return fileRepo.GetByID(family);
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if(_itemRepository != null)
            {
               //dispose
            }
            if(_autodeskFileRepository != null)
            {
                //dispose
            }
        }
        #endregion
    }
}
