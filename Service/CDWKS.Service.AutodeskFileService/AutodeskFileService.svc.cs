using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Xml;
using System.Xml.Serialization;
using CDWKS.Business.AutodeskFileManager;
using CDWKS.Model.EF.BIMXchange;
using CDWKS.Model.EF.FileQueue;
using CDWKS.Model.Poco.Content;
using CDWKS.Shared.ObjectFactory;
using Ninject;
using log4net;
using AutodeskFile = CDWKS.Model.Poco.Content.AutodeskFile;
using AutodeskFileTreeNode = CDWKS.Model.Poco.Content.AutodeskFileTreeNode;
using ContentLibrary = CDWKS.Model.Poco.Content.ContentLibrary;
using Item = CDWKS.Model.Poco.Content.Item;
using Parameter = CDWKS.Model.Poco.Content.Parameter;
using SearchName = CDWKS.Model.Poco.Content.SearchName;
using SearchValue = CDWKS.Model.Poco.Content.SearchValue;
using TreeNode = CDWKS.Model.Poco.Content.TreeNode;


namespace CDWKS.BXC.AutodeskFileService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AutodeskFileService" in code, svc and config file together.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class AutodeskFileService : IAutodeskFileService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(IAutodeskFileService));
        public Dictionary<string, int> FamilyIds { get; set; } 
        public AutodeskFileService()
        {
            log4net.Config.XmlConfigurator.Configure();
            Construction.StandardKernel.Bind<IAutodeskFileManager>().To<AutodeskFileManager>();
            FamilyIds = new Dictionary<string, int>();
        }

        public string IndexAutodeskFile(AutodeskFile file, bool overwrite)
        {
            try
            {
                Construction.StandardKernel.Rebind<IAutodeskFileManager>().To<AutodeskFileManager>();
                     var manager = Construction.StandardKernel.Get<IAutodeskFileManager>();
                     using (var context = new BXCModelEntities())
                     {
                         var result = manager.AddOrUpdateAutodeskFile(GetAutodeskFileFromPoco(file), overwrite, context)
                                          ? Enums.IndexStatus.DataSynced.ToString()
                                          : Enums.IndexStatus.DataSyncFailed.ToString();
                         Log.Info(result);
                         return result;
                     }
            }
            catch (Exception ex)
            {

                Log.Error("Could not Index File", ex);
                return Enums.IndexStatus.DataSyncFailed.ToString();
            }
        }

        public string AddTypeToFile(Item item, string fileName, string owner)
        {
            try
            {
                Construction.StandardKernel.Rebind<IAutodeskFileManager>().To<AutodeskFileManager>();
                var manager = Construction.StandardKernel.Get<IAutodeskFileManager>();
                using (var context = new BXCModelEntities())
                {
                    return manager.AddTypeToFile(GetItemFromPoco(item), fileName, owner, context)
                               ? Enums.IndexStatus.DataSynced.ToString()
                               : Enums.IndexStatus.DataSyncFailed.ToString();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Could not add Type to File", ex);
                return Enums.IndexStatus.DataSyncFailed.ToString();
            }
        }

        public string UpdateLibraryTreeView(string strtree, string owner, string revitVersion)
        {

            var tree = new XmlDocument();
            tree.LoadXml(strtree);
            Construction.StandardKernel.Rebind<IAutodeskFileManager>().To<AutodeskFileManager>();
            var manager = Construction.StandardKernel.Get<IAutodeskFileManager>();
            var serializer = new XmlSerializer(typeof (XmlTree));
            
            var stringReader = new StringReader(tree.OuterXml);
         
            var xmlTree = (XmlTree) serializer.Deserialize(stringReader);
            var library = xmlTree.Root.Name;

            #region Remove Autodesk File Tree Nodes
                                 using (var context = new BXCModelEntities())
                     {
            manager.RemoveAllAutodeskFileTreeNodesForLibrary(library, owner, context);

            manager.RemoveAllTreeNodesForLibrary(library, owner, context);
        }
            #endregion

            CreateTreeNode(null, xmlTree.Root, manager, library);

            return "success";
        }

        private void CreateTreeNode(int? parentId, Folder xmlFolder, IAutodeskFileManager manager, string library)
        {
            using (var context = new BXCModelEntities())
            {
                var nextNode = new Model.EF.BIMXchange.TreeNode();
                if (parentId.HasValue)
                {
                    nextNode.TreeNode1 = manager.GetTreeNode(parentId.Value, context);
                }
                nextNode.Name = xmlFolder.Name;
                nextNode.Library = manager.GetContentLibrary(library, "1", context) ?? manager.AddContentLibrary(library, context);
                var nextId = (manager.AddTreeNode(nextNode, context)).Id;

                foreach (var f in xmlFolder.Folders)
                {
                    CreateTreeNode(nextId, f, manager, library);
                }

                foreach (var family in GetTreeNodeFamilyIds(xmlFolder, new List<int>(), manager, library))
                {
                    var newFileNode = new Model.EF.BIMXchange.AutodeskFileTreeNode
                                          {
                                              AutodeskFile = manager.GetAutodeskFileById(family, context),
                                              TreeNode = manager.GetTreeNode(nextId, context)
                                          };
                    manager.AddAutodeskFileTreeNode(newFileNode, context);
                }
            }
        }


        public AutodeskFile GetAutodeskFile(string fileName, string libraryName)
        {
            try
            {
                Construction.StandardKernel.Rebind<IAutodeskFileManager>().To<AutodeskFileManager>();
                var manager = Construction.StandardKernel.Get<IAutodeskFileManager>();
                using(var context = new BXCModelEntities())
                {
                    return GetAutodeskFileFromDomain(manager.GetAutodeskFile(fileName, libraryName, context));
                }
            }
            catch (Exception ex)
            {
                Log.Error("Could not get Autodesk File", ex);
                return null;
            }

        }


        public List<int> GetTreeNodeFamilyIds(Folder xmlFolder, List<int> ids, IAutodeskFileManager manager, string library )
        {
            ids.AddRange(xmlFolder.Families.Select(family => GetFamilyId(family.Name, manager)).Where(id => id != 0));
            foreach(var folder in xmlFolder.Folders)
            {
                GetTreeNodeFamilyIds(folder, ids, manager, library);
            }
            return ids;
        }

        private int GetFamilyId(string name, IAutodeskFileManager manager)
        {
            if(FamilyIds.ContainsKey(name))
            {
                return FamilyIds[name];
            }
            using (var context = new BXCModelEntities())
            {
                var file = manager.GetAutodeskFile(name, "1", context);
                if (file != null)
                {
                    FamilyIds.Add(name, file.Id);
                    return file.Id;
                }
                return 0;
            }
        }


        public int GetContentLibrary(string libraryName, string ownerId)
        {
            try
            {
                Construction.StandardKernel.Rebind<IAutodeskFileManager>().To<AutodeskFileManager>();
                var manager = Construction.StandardKernel.Get<IAutodeskFileManager>();
                using(var context= new BXCModelEntities())
                {
                    var lib = manager.GetContentLibrary(libraryName, ownerId, context) ?? manager.AddContentLibrary(libraryName, context);
                    return lib.Id;
                }
            }
            catch (Exception)
            {
                //Log.Error("Could not get content libary", ex);
                return 0;
            }

        }

        #region Poco Mappings

        private Model.EF.BIMXchange.AutodeskFile GetAutodeskFileFromPoco(AutodeskFile revitFamily)
        {
            if (revitFamily == null) return null;
            var file = new Model.EF.BIMXchange.AutodeskFile
            {
                Name = revitFamily.Name,
                Version = revitFamily.Version,
                TypeCatalogHeader = revitFamily.TypeCatalogHeader,
                MC_OwnerId = revitFamily.MC_OwnerId,
                Items = GetItemsFromPoco(revitFamily.Items) as EntityCollection<Model.EF.BIMXchange.Item>
            };
            return file;
        }


        private ICollection<Model.EF.BIMXchange.Item> GetItemsFromPoco(ICollection<Item> items)
        {
            if (items == null) return null;
            var serviceItems = new Model.EF.BIMXchange.Item[items.Count];
            var index = 0;
            foreach (var item in items)
            {
                serviceItems[index] = GetItemFromPoco(item);
                index++;
            }
            return serviceItems;
        }

        private Model.EF.BIMXchange.Item GetItemFromPoco(Item item)
        {
            if (item == null) return null;
            var serviceItem = new Model.EF.BIMXchange.Item
            {
                Name = item.Name,
                TypeCatalogEntry = item.TypeCatalogEntry,
                Parameters =
                    (item.Parameters == null
                         ? new Model.EF.BIMXchange.Parameter[0]
                         : GetParametersFromPoco(item.Parameters)) as EntityCollection<Model.EF.BIMXchange.Parameter>
            };
            return serviceItem;
        }

        private ICollection<Model.EF.BIMXchange.Parameter> GetParametersFromPoco(ICollection<Parameter> parameters)
        {
            if (parameters == null) return null;
            var serviceParameters = new Model.EF.BIMXchange.Parameter[parameters.Count];
            var index = 0;
            foreach (var parameter in parameters)
            {
                serviceParameters[index] = GetParameterFromPoco(parameter);
                index++;
            }
            return serviceParameters;
        }

        private Model.EF.BIMXchange.Parameter GetParameterFromPoco(Parameter parameter)
        {
            if (parameter == null) return null;
            var serviceParameter = new Model.EF.BIMXchange.Parameter
            {
                Featured = parameter.Featured,
                Hidden = parameter.Hidden,
                SearchName = GetSearchNameFromPoco(parameter.SearchName),
                SearchValue = GetSearchValueFromPoco(parameter.SearchValue)
            };
            return serviceParameter;
        }

        private Model.EF.BIMXchange.SearchValue GetSearchValueFromPoco(SearchValue searchValue)
        {
            if (searchValue == null) return null;
            var serviceSearchValue = new Model.EF.BIMXchange.SearchValue { Value = searchValue.Value };
            return serviceSearchValue;
        }

        private Model.EF.BIMXchange.SearchName GetSearchNameFromPoco(SearchName searchName)
        {
            if (searchName == null) return null;
            var serviceSearchName = new Model.EF.BIMXchange.SearchName { Name = searchName.Name };
            return serviceSearchName;

        }

        private Model.EF.BIMXchange.AutodeskFileTreeNode GetAutodeskFileTreeNodeFromPoco(AutodeskFileTreeNode treeNode)
        {
            if (treeNode == null) return null;
            var node = new Model.EF.BIMXchange.AutodeskFileTreeNode
            {
                Id = treeNode.Id,
                AutodeskFile = GetAutodeskFileFromPoco(treeNode.AutodeskFile),
                TreeNode = GetTreeNodeFromPoco(treeNode.TreeNode)
            };
            return node;
        }

        private Model.EF.BIMXchange.TreeNode GetTreeNodeFromPoco(TreeNode treeNode)
        {
            if (treeNode== null) return null;
            var tn = new Model.EF.BIMXchange.TreeNode
            {
                Id = treeNode.Id,
                Name = treeNode.Name,
                Library = GetContentLibraryFromPoco(treeNode.ContentLibrary),
                AutodeskFileTreeNodes = GetAutodeskFileTreeNodesFromPoco(treeNode.AutodeskFileTreeNodes) as EntityCollection<Model.EF.BIMXchange.AutodeskFileTreeNode>,
                TreeNode1 = treeNode.TreeNode1 == null ? null : GetTreeNodeFromPoco(treeNode.TreeNode1),
                TreeNodes1 = GetTreeNodesFromPoco(treeNode.TreeNodes1) as EntityCollection<Model.EF.BIMXchange.TreeNode>
            };
            return tn;
        }

        private ICollection<Model.EF.BIMXchange.AutodeskFileTreeNode> GetAutodeskFileTreeNodesFromPoco(ICollection<AutodeskFileTreeNode> autodeskFileTreeNodes)
        {
            if (autodeskFileTreeNodes == null) return null;
            return autodeskFileTreeNodes.Select(pocoAftn => new Model.EF.BIMXchange.AutodeskFileTreeNode
            {
                Id = pocoAftn.Id,
                TreeNode = GetTreeNodeFromPoco(pocoAftn.TreeNode),
                AutodeskFile = GetAutodeskFileFromPoco(pocoAftn.AutodeskFile)
            }).ToList();
        }

        private ICollection<Model.EF.BIMXchange.TreeNode> GetTreeNodesFromPoco(ICollection<TreeNode> treeNodes1)
        {
            return treeNodes1 == null ? null : treeNodes1.Select(GetTreeNodeFromPoco).ToList();
        }

        private Model.EF.BIMXchange.Library GetContentLibraryFromPoco(ContentLibrary contentLibrary)
        {
            if (contentLibrary == null) return null;
            var cl = new Model.EF.BIMXchange.Library
            {
                Id = contentLibrary.Id,
                Name = contentLibrary.Name,
                TreeNodes = GetTreeNodesFromPoco(contentLibrary.TreeNodes) as EntityCollection<Model.EF.BIMXchange.TreeNode>
            };
            return cl;
        }

        #endregion

        #region Domain Mappings
        private ICollection<AutodeskFileTreeNode> GetAutodeskFileTreeNodesFromDomain(
            ICollection<Model.EF.BIMXchange.AutodeskFileTreeNode> domainTreeNodes)
        {
            return domainTreeNodes== null ? null : domainTreeNodes.Select(GetAutodeskFileTreeNodeFromDomain).ToList();
        }

        private AutodeskFileTreeNode GetAutodeskFileTreeNodeFromDomain(
            Model.EF.BIMXchange.AutodeskFileTreeNode aftn)
        {
            if (aftn == null) return null;
            var afTreeNode = new AutodeskFileTreeNode
                                 {
                                     TreeNode = GetTreeNodeFromDomain(aftn.TreeNode),
                                     AutodeskFile = GetAutodeskFileFromDomain(aftn.AutodeskFile),
                                     Id = aftn.Id
                                 };
            return afTreeNode;
        }

        private TreeNode GetTreeNodeFromDomain(Model.EF.BIMXchange.TreeNode treeNodeForLibary,
                                               TreeNode parentNode = null)
        {
            if (treeNodeForLibary == null) return null;
            var tree = new TreeNode
                           {
                               ContentLibrary =null,// GetContentLibraryFromDomain(treeNodeForLibary.ContentLibrary),
                               Id = treeNodeForLibary.Id,
                               Name = treeNodeForLibary.Name,
                               TreeNode1 = null,
                                 //  parentNode == null ? null : GetTreeNodeFromDomain(treeNodeForLibary.TreeNode1),
                               TreeNodes1 =null,// GetTreeNodesFromDomain(treeNodeForLibary.TreeNodes1),
                               AutodeskFileTreeNodes = null
                                 //  GetAutodeskFileTreeNodesFromDomain(treeNodeForLibary.AutodeskFileTreeNodes)
                           };
            return tree;
        }

        private ContentLibrary GetContentLibraryFromDomain(Model.EF.BIMXchange.Library contentLibrary)
        {
            if (contentLibrary == null) return null;
            var contLib = new ContentLibrary
                              {
                                  Id = contentLibrary.Id,
                                  Name = contentLibrary.Name,
                                  TreeNodes = GetTreeNodesFromDomain(contentLibrary.TreeNodes)
                              };
            return contLib;
        }

        private ICollection<TreeNode> GetTreeNodesFromDomain(
            ICollection<Model.EF.BIMXchange.TreeNode> treeNodes)
        {
            if (treeNodes == null) return null;
            var nodes = treeNodes.Select(domainNode => new TreeNode
                                                           {
                                                               Id = domainNode.Id,
                                                               Name = domainNode.Name,
                                                               ContentLibrary =
                                                                   GetContentLibraryFromDomain(domainNode.Library),
                                                               AutodeskFileTreeNodes =
                                                                   GetAutodeskFileTreeNodesFromDomain(
                                                                       domainNode.AutodeskFileTreeNodes)
                                                           }).ToList();

            return nodes;
        }

        private AutodeskFile GetAutodeskFileFromDomain(Model.EF.BIMXchange.AutodeskFile autodeskFile)
        {
            if (autodeskFile == null) return null;
            var file = new AutodeskFile
                           {
                               Id = autodeskFile.Id,
                               Name = autodeskFile.Name,
                               TypeCatalogHeader = autodeskFile.TypeCatalogHeader,
                               Version = autodeskFile.Version,
                               Items = GetItemsFromDomain(autodeskFile.Items)
                           };
            return file;
        }

        private ICollection<Item> GetItemsFromDomain(ICollection<Model.EF.BIMXchange.Item> items)
        {
            return items == null ? null : items.Select(GetItemFromDomain).ToList();
        }

        private Item GetItemFromDomain(Model.EF.BIMXchange.Item item)
        {
            if (item == null) return null;
            var pocoItem = new Item
                               {
                                   Id = item.Id,
                                   Name = item.Name,
                                   Parameters = GetParametersFromDomain(item.Parameters),
                                   TypeCatalogEntry = item.TypeCatalogEntry,
                                   AutodeskFile = GetAutodeskFileFromDomain(item.AutodeskFile)
                               };
            return pocoItem;
        }

        private ICollection<Parameter> GetParametersFromDomain(
            ICollection<Model.EF.BIMXchange.Parameter> parameters)
        {
            return parameters == null ? null : parameters.Select(GetParameterFromDomain).ToList();
        }

        private Parameter GetParameterFromDomain(Model.EF.BIMXchange.Parameter domainParam)
        {
            if (domainParam == null) return null;
            var param = new Parameter
                            {
                                Id = domainParam.Id,
                                Featured = domainParam.Featured,
                                Hidden = domainParam.Hidden,
                                Item = GetItemFromDomain(domainParam.Item),
                                SearchName = GetSearchNameFromDomain(domainParam.SearchName),
                                SearchValue = GetSearchValueFromDomain(domainParam.SearchValue)
                            };
            return param;

        }

        private SearchValue GetSearchValueFromDomain(Model.EF.BIMXchange.SearchValue searchValue)
        {
            return searchValue == null ? null : new SearchValue {Id = searchValue.Id, Value = searchValue.Value};
        }

        private SearchName GetSearchNameFromDomain(Model.EF.BIMXchange.SearchName searchName)
        {
            return searchName == null ? null : new SearchName {Id = searchName.Id, Name = searchName.Name};
        }

        #endregion
    }


}
