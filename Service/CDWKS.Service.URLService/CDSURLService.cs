using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using CDWKS.Model.EF.Content;
using CDWKS.Repository.Content;
using CDWKS.Shared.ObjectFactory;
using Ionic.Zip;
using Ninject;

namespace CDWKS.Service.URLService
{
    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    // NOTE: If the service is renamed, remember to update the global.asax.cs file
    public class CDSURLService
    {
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

        [WebGet(UriTemplate = "?domain={Manufacturer}&pn={ProductNumber}", ResponseFormat = WebMessageFormat.Json)]
        public ResponseObject GetURL(string manufacturer, string productNumber)
        {
            var response = new ResponseObject();
            response.ModelNo = productNumber;
            using (var model = new CDSModelContainer())
            {
                var item =
                    (from o in model.CDSLinks
                     where o.CDSManufacturer == manufacturer & o.CDSProdNum == productNumber
                     select o).ToList();
                if (item.Any())
                {
                    
                    response.FoundInENGworksDB = true;
                    var firstOrDefault = item.FirstOrDefault();
                    if (firstOrDefault != null)
                    {
                        response.RevitFamilyName = item.First().Family;
                        var url = firstOrDefault.Url;
                        response.Message =
                            "Provided Link will download Zip package containing .rfa and if family is a type catalog a .txt file will be included";
                        if (String.IsNullOrWhiteSpace(url))
                        {
                            var path = CreateDownload(item);
                            response.URL = path;
                            if (path != "Found in CDS Link but not in ENGworks Library")
                            {
                                foreach (var i in item)
                                {
                                    i.Url = path;
                                }
                                model.SaveChanges();
                            }
                        }
                        else
                        {
                            response.URL = url;
                        }
                    }
                    return response;
                }
                response.FoundInENGworksDB = false;
                response.Message = "Item with Manufacturer: " + manufacturer + " and Product Number " +
                                   productNumber + " could not be found.";
                return response;
            }
        }

        [WebGet(UriTemplate = "/collection/?domain={Manufacturer}&pn={ProductNumbers}", ResponseFormat = WebMessageFormat.Json)]
        public ResponseCollectionObject GetURLCollection(string manufacturer, string productNumbers)
        {
            var response = new ResponseCollectionObject();
            var collection = new List<ResponseObject>();
            using (var model = new CDSModelContainer())
            {
                var productNumberCollection = productNumbers.Split(',');
                foreach (var productNumber in productNumberCollection)
                {
                    var productResponse = new ResponseObject();
                    productResponse.ModelNo = productNumber;
                    var item =
                        (from o in model.CDSLinks
                         where o.CDSManufacturer == manufacturer & o.CDSProdNum == productNumber
                         select o).ToList();
                    if (item.Any())
                    {
                        productResponse.RevitFamilyName = item.First().Family;
                        productResponse.FoundInENGworksDB = false;
                        var firstOrDefault = item.FirstOrDefault();
                        if (firstOrDefault != null)
                        {
                            var url = firstOrDefault.Url;
                            productResponse.Message =
                                "Provided Link will download Zip package containing .rfa and if family is a type catalog a .txt file will be included";
                            if (String.IsNullOrWhiteSpace(url))
                            {
                                var path = CreateDownload(item);
                                productResponse.URL = path;
                                if (path != "Found in CDS Link but not in ENGworks Library")
                                {
                                    foreach (var i in item)
                                    {
                                        i.Url = path;
                                    }
                                    model.SaveChanges();
                                }
                            }
                            else
                            {
                                productResponse.URL = url;
                            }
                        }

                    }
                    else
                    {
                        productResponse.FoundInENGworksDB = false;
                        productResponse.Message =  "Item with Manufacturer: " + manufacturer + " and Product Number " +
                                   productNumber + " could not be found.";
                        productResponse.URL = string.Empty;
                    }
                    collection.Add(productResponse);
                }
                response.Objects = collection;
                return response;
            }
        }

        private string CreateDownload(List<CDSLink> items)
        {
            
            var itemIds = items.Select(i => ItemRepository.GetByNameAndAutodeskFileName(i.Family, i.ItemType, "1")).ToList();
            return CreateDownloadPackage(itemIds);
        }


        public string CreateDownloadPackage(List<Item> items)
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
                    var distinctFamilies = (from f in items select f.AutodeskFile.Id).Distinct();
                    var count = distinctFamilies.Count();
                    if (count == 0)
                        return "Found in CDS Link but not in ENGworks Library";
                    foreach (var famId in distinctFamilies)
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

        public static void AddtoZip(string filePath, string fileName, string zipFileName, string zipFilePath)
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
    }
}