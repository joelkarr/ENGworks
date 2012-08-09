using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using CDWKS.Business.AutodeskFileManager;
using CDWKS.Model.EF.Content;
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

namespace CDWKS.Utility.TreeViewCreator
{
    class Program
    {
        //private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        public static Dictionary<string, int> FamilyIds { get; set; } 
        static void Main(string[] args)
        {
            FamilyIds = new Dictionary<string, int>();
            var files = Directory.GetFiles(@"C:\FTP\TreeViews", "*.xml");
            foreach (var file in files)
            {
                var tree = new XmlDocument();
                tree.Load(file);
                try
                {
                    UpdateLibraryTreeView(tree, "1", "2011");
                    File.Copy(file,file.Replace(@"FTP\TreeViews",@"FTP\TreeViews\Archive"));
                    File.Delete(file);
                }
                catch(Exception ex)
                {
                    //Log.Error("Tree Failed", ex);
                }
                
            }
           
        }

        public static string UpdateLibraryTreeView(XmlDocument tree, string owner, string revitVersion)
        {
            try
            {
                Construction.StandardKernel.Rebind<IAutodeskFileManager>().To<AutodeskFileManager>();
                var manager = Construction.StandardKernel.Get<IAutodeskFileManager>();
                var serializer = new XmlSerializer(typeof (XmlTree));

                var stringReader = new StringReader(tree.OuterXml);

                var xmlTree = (XmlTree) serializer.Deserialize(stringReader);
                var library = xmlTree.Root.Name;

                #region Remove Autodesk File Tree Nodes
                using (var context = new BXC_ContentModelEntities())
                {
                    manager.RemoveAllAutodeskFileTreeNodesForLibrary(library, owner,context);

                    manager.RemoveAllTreeNodesForLibrary(library, owner, context);
                }

                #endregion

                CreateTreeNode(null, xmlTree.Root, manager, library);

                return "success";
            }

            catch (Exception ex)
            {
                //Log.Error("UpdateLibraryTreeView Failed", ex);
                return "failed";
            }
        }

        private static void CreateTreeNode(int? parentId, Folder xmlFolder, IAutodeskFileManager manager, string library)
        {
            try
            {
                using (var context = new BXC_ContentModelEntities())
                {
                    var nextNode = new Model.EF.Content.TreeNode();
                    if (parentId.HasValue)
                    {
                        nextNode.TreeNode1 = manager.GetTreeNode(parentId.Value, context);
                    }
                    nextNode.Name = xmlFolder.Name;
                    nextNode.ContentLibrary = manager.GetContentLibrary(library, "1", context) ??
                                              manager.AddContentLibrary(library,context);
                    var n = manager.AddTreeNode(nextNode, context);



                    if(n == null)
                    {
                       var s= "catch";
                    }
                    var nextId = n == null ? 0 : n.Id;
                    foreach (var family in GetTreeNodeFamilyIds(xmlFolder, new List<int>(), manager, library))
                    {
                        var newFileNode = new Model.EF.Content.AutodeskFileTreeNode
                        {
                            AutodeskFile = manager.GetAutodeskFileById(family, context),
                            TreeNode = manager.GetTreeNode(nextId, context)
                        };
                        manager.AddAutodeskFileTreeNode(newFileNode, context);
                    }
                    foreach (var f in xmlFolder.Folders)
                    {
                        CreateTreeNode(nextId, f, manager, library);
                    }


                }
            }

            catch (Exception ex)
            {
                var cat = "";
                //  Log.Error("CreateNode Failed", ex);

            }

        }

        public static List<int> GetTreeNodeFamilyIds(Folder xmlFolder, List<int> ids, IAutodeskFileManager manager, string library)
        {
            ids.AddRange(xmlFolder.Families.Select(family => GetFamilyId(family.Name, manager)).Where(id => id != 0));
            foreach (var folder in xmlFolder.Folders)
            {
                GetTreeNodeFamilyIds(folder, ids, manager, library);
            }
            return ids;
        }

        private static int GetFamilyId(string name, IAutodeskFileManager manager)
        {
            if (FamilyIds.ContainsKey(name))
            {
                return FamilyIds[name];
            }
            using (var context = new BXC_ContentModelEntities())
            {
                var file = manager.GetAutodeskFile(name, "1",context);
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
                using (var context = new BXC_ContentModelEntities())
                {
                    Construction.StandardKernel.Rebind<IAutodeskFileManager>().To<AutodeskFileManager>();

                    var manager = Construction.StandardKernel.Get<IAutodeskFileManager>();
                    {
                        var lib = manager.GetContentLibrary(libraryName, ownerId, context) ??
                                  manager.AddContentLibrary(libraryName, context);
                        return lib.Id;
                    }
                }

            }
            catch (Exception)
            {
                //Log.Error("Could not get content libary", ex);
                return 0;
            }

        }

        #region Poco Mappings

        private Model.EF.Content.AutodeskFile GetAutodeskFileFromPoco(AutodeskFile revitFamily)
        {
            if (revitFamily == null) return null;
            var file = new Model.EF.Content.AutodeskFile
            {
                Name = revitFamily.Name,
                Version = revitFamily.Version,
                TypeCatalogHeader = revitFamily.TypeCatalogHeader,
                MC_OwnerId = revitFamily.MC_OwnerId,
                Items = GetItemsFromPoco(revitFamily.Items) as EntityCollection<Model.EF.Content.Item>
            };
            return file;
        }


        private ICollection<Model.EF.Content.Item> GetItemsFromPoco(ICollection<Item> items)
        {
            if (items == null) return null;
            var serviceItems = new Model.EF.Content.Item[items.Count];
            var index = 0;
            foreach (var item in items)
            {
                serviceItems[index] = GetItemFromPoco(item);
                index++;
            }
            return serviceItems;
        }

        private Model.EF.Content.Item GetItemFromPoco(Item item)
        {
            if (item == null) return null;
            var serviceItem = new Model.EF.Content.Item
            {
                Name = item.Name,
                TypeCatalogEntry = item.TypeCatalogEntry,
                Parameters =
                    (item.Parameters == null
                         ? new Model.EF.Content.Parameter[0]
                         : GetParametersFromPoco(item.Parameters)) as EntityCollection<Model.EF.Content.Parameter>
            };
            return serviceItem;
        }

        private ICollection<Model.EF.Content.Parameter> GetParametersFromPoco(ICollection<Parameter> parameters)
        {
            if (parameters == null) return null;
            var serviceParameters = new Model.EF.Content.Parameter[parameters.Count];
            var index = 0;
            foreach (var parameter in parameters)
            {
                serviceParameters[index] = GetParameterFromPoco(parameter);
                index++;
            }
            return serviceParameters;
        }

        private Model.EF.Content.Parameter GetParameterFromPoco(Parameter parameter)
        {
            if (parameter == null) return null;
            var serviceParameter = new Model.EF.Content.Parameter
            {
                Featured = parameter.Featured,
                Hidden = parameter.Hidden,
                SearchName = GetSearchNameFromPoco(parameter.SearchName),
                SearchValue = GetSearchValueFromPoco(parameter.SearchValue)
            };
            return serviceParameter;
        }

        private Model.EF.Content.SearchValue GetSearchValueFromPoco(SearchValue searchValue)
        {
            if (searchValue == null) return null;
            var serviceSearchValue = new Model.EF.Content.SearchValue { Value = searchValue.Value };
            return serviceSearchValue;
        }

        private Model.EF.Content.SearchName GetSearchNameFromPoco(SearchName searchName)
        {
            if (searchName == null) return null;
            var serviceSearchName = new Model.EF.Content.SearchName { Name = searchName.Name };
            return serviceSearchName;

        }
        private Model.EF.Content.TreeNode GetTreeNodeFromPoco(TreeNode treeNode)
        {
            if (treeNode == null) return null;
            var tn = new Model.EF.Content.TreeNode
            {
                Id = treeNode.Id,
                Name = treeNode.Name,
                ContentLibrary = GetContentLibraryFromPoco(treeNode.ContentLibrary),
                AutodeskFileTreeNodes = GetAutodeskFileTreeNodesFromPoco(treeNode.AutodeskFileTreeNodes) as EntityCollection<Model.EF.Content.AutodeskFileTreeNode>,
                TreeNode1 = treeNode.TreeNode1 == null ? null : GetTreeNodeFromPoco(treeNode.TreeNode1),
                TreeNodes1 = GetTreeNodesFromPoco(treeNode.TreeNodes1) as EntityCollection<Model.EF.Content.TreeNode>
            };
            return tn;
        }

        private ICollection<Model.EF.Content.AutodeskFileTreeNode> GetAutodeskFileTreeNodesFromPoco(ICollection<AutodeskFileTreeNode> autodeskFileTreeNodes)
        {
            if (autodeskFileTreeNodes == null) return null;
            return autodeskFileTreeNodes.Select(pocoAftn => new Model.EF.Content.AutodeskFileTreeNode
            {
                Id = pocoAftn.Id,
                TreeNode = GetTreeNodeFromPoco(pocoAftn.TreeNode),
                AutodeskFile = GetAutodeskFileFromPoco(pocoAftn.AutodeskFile)
            }).ToList();
        }

        private ICollection<Model.EF.Content.TreeNode> GetTreeNodesFromPoco(ICollection<TreeNode> treeNodes1)
        {
            return treeNodes1 == null ? null : treeNodes1.Select(GetTreeNodeFromPoco).ToList();
        }

        private Model.EF.Content.ContentLibrary GetContentLibraryFromPoco(ContentLibrary contentLibrary)
        {
            if (contentLibrary == null) return null;
            var cl = new Model.EF.Content.ContentLibrary
            {
                Id = contentLibrary.Id,
                Name = contentLibrary.Name,
                TreeNodes = GetTreeNodesFromPoco(contentLibrary.TreeNodes) as EntityCollection<Model.EF.Content.TreeNode>
            };
            return cl;
        }

        #endregion

        #region Domain Mappings
        private ICollection<AutodeskFileTreeNode> GetAutodeskFileTreeNodesFromDomain(
            ICollection<Model.EF.Content.AutodeskFileTreeNode> domainTreeNodes)
        {
            return domainTreeNodes == null ? null : domainTreeNodes.Select(GetAutodeskFileTreeNodeFromDomain).ToList();
        }

        private AutodeskFileTreeNode GetAutodeskFileTreeNodeFromDomain(
            Model.EF.Content.AutodeskFileTreeNode aftn)
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

        private TreeNode GetTreeNodeFromDomain(Model.EF.Content.TreeNode treeNodeForLibary,
                                               TreeNode parentNode = null)
        {
            if (treeNodeForLibary == null) return null;
            var tree = new TreeNode
            {
                ContentLibrary = null,// GetContentLibraryFromDomain(treeNodeForLibary.ContentLibrary),
                Id = treeNodeForLibary.Id,
                Name = treeNodeForLibary.Name,
                TreeNode1 = null,
                //  parentNode == null ? null : GetTreeNodeFromDomain(treeNodeForLibary.TreeNode1),
                TreeNodes1 = null,// GetTreeNodesFromDomain(treeNodeForLibary.TreeNodes1),
                AutodeskFileTreeNodes = null
                //  GetAutodeskFileTreeNodesFromDomain(treeNodeForLibary.AutodeskFileTreeNodes)
            };
            return tree;
        }

        private ContentLibrary GetContentLibraryFromDomain(Model.EF.Content.ContentLibrary contentLibrary)
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
            ICollection<Model.EF.Content.TreeNode> treeNodes)
        {
            if (treeNodes == null) return null;
            var nodes = treeNodes.Select(domainNode => new TreeNode
            {
                Id = domainNode.Id,
                Name = domainNode.Name,
                ContentLibrary =
                    GetContentLibraryFromDomain(domainNode.ContentLibrary),
                AutodeskFileTreeNodes =
                    GetAutodeskFileTreeNodesFromDomain(
                        domainNode.AutodeskFileTreeNodes)
            }).ToList();

            return nodes;
        }

        private AutodeskFile GetAutodeskFileFromDomain(Model.EF.Content.AutodeskFile autodeskFile)
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

        private ICollection<Item> GetItemsFromDomain(ICollection<Model.EF.Content.Item> items)
        {
            return items == null ? null : items.Select(GetItemFromDomain).ToList();
        }

        private Item GetItemFromDomain(Model.EF.Content.Item item)
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
            ICollection<Model.EF.Content.Parameter> parameters)
        {
            return parameters == null ? null : parameters.Select(GetParameterFromDomain).ToList();
        }

        private Parameter GetParameterFromDomain(Model.EF.Content.Parameter domainParam)
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

        private SearchValue GetSearchValueFromDomain(Model.EF.Content.SearchValue searchValue)
        {
            return searchValue == null ? null : new SearchValue { Id = searchValue.Id, Value = searchValue.Value };
        }

        private SearchName GetSearchNameFromDomain(Model.EF.Content.SearchName searchName)
        {
            return searchName == null ? null : new SearchName { Id = searchName.Id, Name = searchName.Name };
        }

        #endregion

    }
}
