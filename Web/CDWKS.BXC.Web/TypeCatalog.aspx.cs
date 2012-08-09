using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using CDWKS.Model.EF.Content;
using CDWKS.Model.EF.MasterControl;
using Ionic.Zip;
using Telerik.Web.UI;

namespace CDWKS.BXC.Web
{
    public partial class TypeCatalog : Page
    {
// ReSharper disable InconsistentNaming
        protected void Page_Load(object sender, EventArgs e)
// ReSharper restore InconsistentNaming
        {
            //TODO: switch to ajax data binding and added loading panel
            if (!IsPostBack)
            {
                var id = Request.QueryString["id"];
                if (id != null)
                {
                    var familyId = Int32.Parse(id);
                    grdTypes.DataSource = GetTypeCatalogData(familyId);
                    grdTypes.DataBind();
                    foreach (GridColumn col in grdTypes.Columns)
                    {
                        col.CurrentFilterFunction = GridKnownFunction.Contains;
                        col.ShowFilterIcon = false;
                    }
                    var column = grdTypes.MasterTableView.GetColumnSafe("ID");
                    if (column != null)
                    {
                        column.Visible = !column.Visible;
                        grdTypes.Rebind();
                    }
                }
            }
        }

        protected void GrdTypesNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var id = Request.QueryString["id"];
            if (id == null) return;
            var familyId = Int32.Parse(id);
            grdTypes.DataSource = GetTypeCatalogData(familyId);
        }


        [WebMethod]
        public static string CreateDownloadPackage(object ids)
        {
            var itemIds = (Dictionary<string, object>) ids;
            var guid = Guid.NewGuid().ToString();
            var id = Int32.Parse(itemIds["0"].ToString());
            try
            {
                var zipFilePath = String.Format("{0}\\{1}\\",
                                                   ConfigurationManager.AppSettings["DownloadDirectory"],
                                                   guid);
                var zipFileName = String.Format("{0}{1}",
                                                   zipFilePath,
                                                   "package.zip");
                using (var container = new BXC_ContentModelEntities())
                {
                    var family = (from c in container.Items
                                  where c.Id == id
                                  select new
                                             {
                                                 FileName = c.AutodeskFile.Name,
                                                 c.AutodeskFile.TypeCatalogHeader,
                                                 TypeCatalogLine = c.TypeCatalogEntry,
                                                 Owner = c.AutodeskFile.MC_OwnerId
                                             }).FirstOrDefault();

                    if (family != null)
                    {
                        var filePath = String.Format("{0}\\{1}.rfa",
                                                     ConfigurationManager.AppSettings["ContentDirectory"],
                                                     family.FileName);
                        AddtoZip(filePath, family.FileName, zipFileName, zipFilePath);
                    }
                    if (family != null && !String.IsNullOrWhiteSpace(family.TypeCatalogHeader))
                    {
                        var typeCatalogPath = String.Format("{0}\\{1}\\{2}",
                                                               ConfigurationManager.AppSettings["DownloadDirectory"],
                                                               guid,
                                                               family.FileName + ".txt");

                        var sw = File.CreateText(typeCatalogPath);
                        {
                            sw.WriteLine(family.TypeCatalogHeader);
                            foreach (var i in itemIds.Select(id2 => Int32.Parse(id2.Value.ToString())).Select(iD => (from t in container.Items where t.Id == id select t.TypeCatalogEntry).
                                                                                                                        FirstOrDefault()))
                            {
                                sw.WriteLine(i);
                            }
                        }

                        sw.Close();
                        AddtoZip(typeCatalogPath, family.FileName + ".txt", zipFileName, zipFilePath);
                    }
                }
                try
                {
                    using (var container = new BXC_MasterControlEntities())
                    {
                        foreach (var id2 in itemIds)
                        {
                            var download = new Download {DateTime = DateTime.Now, Content_Id = id2.ToString()};
                            var libName = HttpContext.Current.Session["LibraryName"].ToString();
                            var lib =
                                (from l in container.Libraries where l.Name == libName select l).FirstOrDefault();
                            download.Library = lib;
                            var uname = HttpContext.Current.Session["UserName"].ToString();
                            download.User =
                                (from u in container.Users where u.UserName == uname select u).FirstOrDefault();
                            container.SaveChanges();
                        }
                    }
                }
                catch
                {
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
                        zip.AddFile(filePath, string.Empty);
                    }
                }
                zip.Save();
            }
        }

        public DataTable GetTypeCatalogData(int familyId)
        {
            string header;
            var rows = new List<string>();
            using (var entities = new BXC_ContentModelEntities())
            {
                header = (from f in entities.AutodeskFiles where f.Id == familyId select f.TypeCatalogHeader).First();
                var types = from t in entities.Items where t.AutodeskFile.Id == familyId select t;
                rows.AddRange(types.Select(type => type.Id + "," + type.TypeCatalogEntry));
            }

            var dtTypeCatalog = new DataTable();
            var paramNames = header.Split(',');
            var index = 0;
            foreach (var splits in paramNames.Select(input => input.Split('#')))
            {
                paramNames[index] = splits[0];
                index++;
            }

            dtTypeCatalog.Columns.Add("ID");
            foreach (var input in paramNames)
            {
                if (input == String.Empty)
                {
                    dtTypeCatalog.Columns.Add("Type");
                }
                else
                {
                    var temp = input.Replace("(", "");
                    temp = temp.Replace(")", "");
                    dtTypeCatalog.Columns.Add(temp);
                }
            }

            foreach (var row in rows)
            {
                try
                {
                    //TODO: extra commas in rows.
                    var rowValues = row.Split(',');
                    dtTypeCatalog.Rows.Add(rowValues);
                }
                catch
                {
                }
            }
            return dtTypeCatalog;
        }

        protected void BtnDownloadClick(object sender, ImageClickEventArgs e)
        {
            var itemIds = (from GridItem i in grdTypes.SelectedItems select int.Parse(i.OwnerTableView.DataKeyValues[i.DataSetIndex]["Id"].ToString())).ToList();
            if (itemIds.Count() > 0)
            {
                CreateDownloadPackage(itemIds);
            }
        }
    }
}