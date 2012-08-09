using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using CDWKS.Model.EF.Content;
using CDWKS.Model.EF.MasterControl;
using Ionic.Zip;
using Telerik.Web.UI;

namespace CDWKS.BXC.Web
{
    public partial class Default : Page
    {
        #region Session 

        public string LibraryName
        {
            get
            {
                return Session["LibraryName"] != null ? Session["LibraryName"].ToString() : "Default";
            }
            set { Session["LibraryName"] = value; }
        }

        public string UserName
        {
            get {
                return Session["UserName"] != null ? Session["UserName"].ToString() : "Default";
            }
            set { Session["UserName"] = value; }
        }

        public Int32 NodeId
        {
            get
            {
                return Session["NodeId"] != null ? Convert.ToInt32(Session["NodeId"]) : 0;
            }
            set { Session["NodeId"] = value; }
        }

        public Int32 Skip
        {
            get {
                return Session["Skip"] != null ? Convert.ToInt32(Session["Skip"]) : 0;
            }
            set { Session["Skip"] = value; }
        }

        public Int32 Take
        {
            get {
                return Session["Take"] != null ? Convert.ToInt32(Session["Take"]) : 5;
            }
            set { Session["Take"] = value; }
        }

        public List<Filter> Filters
        {
            get { return Session["Filters"] != null ? (List<Filter>) (Session["Filters"]) : new List<Filter>(); }
            set { Session["Filters"] = value; }
        }

        #endregion

// ReSharper disable InconsistentNaming
        protected void Page_Load(object sender, EventArgs e)
// ReSharper restore InconsistentNaming
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["r"] != null)
                {
                    Session["Revit"] = 1;
                }
                if (Session["User"] != null && Session["Pass"] != null)
                {
                    var userName = Session["User"].ToString();
                    var pass = Session["Pass"].ToString();
                    windowLogin.VisibleOnPageLoad = !UserLogin(userName, pass, false);
                }
                else if (Request.QueryString["u"] != null)
                {
                    //if username sent then from Revit App
                    var windowsLogin = Request.QueryString["u"];
                    Session["Alias"] = windowsLogin;
                    windowLogin.VisibleOnPageLoad = !UserLogin(windowsLogin, string.Empty, true);
                }
                else
                {
                    //user entered from web browser - show login panel
                    windowLogin.VisibleOnPageLoad = true;
                }
            }
            if (Session["UpdateLibrary"] != null)
            {
                var userName = Session["User"].ToString();
                using (var masterControl = new BXC_MasterControlEntities())
                {
                    var lib = (from l in masterControl.ExtendedProperties
                                  where l.User.UserName == userName & l.PropertyName.Name == "CurrentLibrary"
                                  select l.PropertyValue.Value).FirstOrDefault();
                    if (lib != null)
                    {
                        LibraryName = lib;
                        LoadData(true, true);
                        Session["UpdateLibrary"] = null;
                    }
                }
            }
        }

        #region Events

        #region Button Clicks

        protected void BtnRemoveAllClick(object sender, EventArgs e)
        {
            Filters = new List<Filter>();
            lbFilters.DataSource = Filters;
            lbFilters.DataBind();
            LoadData(false, true);
            pnlRemoveFilters.Visible = false;
        }

        protected void BtnAddFilterClick(object sender, EventArgs e)
        {
            //TODO: Validate the fields are populated
            if (Filters.Count() < 5)
            {
                var temp = Filters;
                temp.Add(rdbtnFilter.SelectedIndex == 0
                             ? new Filter("Any Parameter", "Contains", txtValue.Text)
                             : new Filter(cbFields.Text, ddOperators.SelectedValue, txtValue.Text));
                Filters = temp;
                lbFilters.DataSource = Filters;
                lbFilters.DataBind();
                LoadData(false, true);
            }
            else
            {
                lbFilters.DataSource = Filters;
                lbFilters.DataBind();
                //Show message that filter limit reached
                pnlMaxReahed.Visible = true;
            }
            pnlRemoveFilters.Visible = Filters.Count > 1;
        }

        protected void BtnRemoveClick(object sender, EventArgs e)
        {
            var btn = (ImageButton) sender;
            var temp = Filters;
            if (btn.CommandArgument == "All")
            {
                temp.RemoveAll(a => a.Value != "zzz");
            }
            else
            {
                var filterId = new Guid(btn.CommandArgument);
                var filter = (from f in temp where f.Id == filterId select f).FirstOrDefault();
                temp.Remove(filter);
            }
            Filters = temp;
            lbFilters.DataSource = Filters;
            lbFilters.DataBind();
            //Show message that filter limit reached
            pnlMaxReahed.Visible = false;
            LoadData(false, true);
            if (Filters.Count > 2)
            {
                pnlRemoveFilters.Visible = false;
            }
        }

        protected void BtnTreeTabClick(object sender, EventArgs e)
        {
            rmpNavigation.SelectedIndex = 0;
        }

        protected void BtnFilterTabClick(object sender, EventArgs e)
        {
            rmpNavigation.SelectedIndex = 1;
        }

        #endregion

        #region Tree View Events

        protected void TvDirectoryNodeClick(object sender, RadTreeNodeEventArgs e)
        {
            int id;
            if (!Int32.TryParse(e.Node.Value, out id)) return;
            Filters = new List<Filter>();
            NodeId = id;
            Skip = 0;
            grdItems.CurrentPageIndex = 0;
            LoadData(false, true);
        }

        protected void TvDirectoryNodeExpand(object sender, RadTreeNodeEventArgs e)
        {
            int parentId;

            if (Int32.TryParse(e.Node.Value, out parentId))
            {
                using (var container = new BXC_ContentModelEntities())
                {
                    e.Node.Nodes.Clear();
                    var nodes = from n in container.TreeNodes
                                where n.TreeNode1.Id == parentId
                                select new {text = n.Name, value = n.Id, count = n.TreeNodes1.Count};
                    foreach (var n in nodes)
                    {
                        var node = new RadTreeNode
                                       {
                                           Text = n.text,
                                           Value = n.value.ToString(),
                                           ExpandMode =
                                               n.count == 0
                                                   ? TreeNodeExpandMode.ClientSide
                                                   : TreeNodeExpandMode.ServerSide
                                       };
                        e.Node.Nodes.Add(node);
                    }
                }
            }
            e.Node.Expanded = true;
        }

        #endregion

        #region Selection Changed

        protected void GrdItemsPageIndexChanged(object sender, GridPageChangedEventArgs e)
        {
            Skip = Take*e.NewPageIndex;
            LoadData(false, false);
        }

        protected void RdFilterChanged(object sender, EventArgs e)
        {
            var rbList = (RadioButtonList) sender;
            pnlAdvancedFilter.Visible = rbList.SelectedIndex != 0;
        }

        #endregion

        protected void GrdItemsItemBound(object sender, GridItemEventArgs e)
        {
        }

        #endregion

        #region Methods

        public bool UserLogin(string userName, string password, bool aliasLogin)
        {
            Session["Filters"] = null;
            using (var masterControl = new BXC_MasterControlEntities())
            {
                if (aliasLogin)
                {
                    var aliasUser = (from u in masterControl.Users
                                      where
                                          u.ExtendedProperties.Any(
                                              p => p.PropertyName.Name == "Alias" && p.PropertyValue.Value == userName)
                                      select u).FirstOrDefault();
                    if (aliasUser != null)
                    {
                        Session["User"] = aliasUser.UserName;
                        Session["Pass"] = aliasUser.Password;
                        //user Exists - load current library
                        var lib = (from l in masterControl.ExtendedProperties
                                      where
                                          l.User.UserName == aliasUser.UserName &
                                          l.PropertyName.Name == "CurrentLibrary"
                                      select l.PropertyValue.Value).FirstOrDefault();
                        if (lib != null)
                        {
                            LibraryName = lib;
                            LoadData(true, true);


                            //FUTURE TODO: Load recent downloads and favorites
                        }
                        return true;
                    }
                }
                else
                {
                    var user =
                        (from u in masterControl.Users where u.UserName == userName select u.UserName).FirstOrDefault();
                    if (!String.IsNullOrWhiteSpace(user))
                    {
                        Session["User"] = user;
                        Session["Pass"] = password;
                        var pass =
                            (from u in masterControl.Users where u.UserName == userName select u.Password).
                                FirstOrDefault();
                        if (password == pass)
                        {
                            //user Exists - load current library
                            var lib = (from l in masterControl.ExtendedProperties
                                          where l.User.UserName == user & l.PropertyName.Name == "CurrentLibrary"
                                          select l.PropertyValue.Value).FirstOrDefault();
                            if (lib != null)
                            {
                                LibraryName = lib;
                                LoadData(true, true);


                                //FUTURE TODO: Load recent downloads and favorites
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        [WebMethod]
        public static RadComboBoxData GetFieldNames(RadComboBoxContext context)
        {
            var fields = new List<string>();
            if (HttpContext.Current.Session["Fields"] != null)
            {
                fields = (List<string>) HttpContext.Current.Session["Fields"];
            }
            var data = (from f in fields where f.ToLower().Contains(context.Text.ToLower()) select f).ToList();
            var comboData = new RadComboBoxData();
            var itemOffset = context.NumberOfItems;
            var endOffset = Math.Min(itemOffset + 10, data.Count);
            comboData.EndOfItems = endOffset == data.Count;

            var result = new List<RadComboBoxItemData>(endOffset - itemOffset);

            for (var i = itemOffset; i < endOffset; i++)
            {
                var itemData = new RadComboBoxItemData {Text = data.ElementAt(i), Value = data.ElementAt(i)};

                result.Add(itemData);
            }

            comboData.Message = GetStatusMessage(endOffset, data.Count);

            comboData.Items = result.ToArray();
            return comboData;
        }

        private static string GetStatusMessage(int offset, int total)
        {
            return total <= 0 ? "No matches" : String.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", offset, total);
        }

        [WebMethod]
        public void LoadData(bool updateLibrary, bool updateSearchNames)
        {
            using (var container = new BXC_ContentModelEntities())
            {
                //Update Tree if Needed
                if (updateLibrary)
                {
                    //Update Tree View
                    var rootnode =
                        (from t in container.TreeNodes
                         where t.ContentLibrary.Name == LibraryName & t.TreeNode1 == null
                         select t).FirstOrDefault();

                    tvDirectory.Nodes.Clear();
                    if (rootnode != null)
                    {
                        var root = new RadTreeNode {Text = rootnode.Name, Value = rootnode.Id.ToString()};
                        NodeId = rootnode.Id;
                        root.ExpandMode = TreeNodeExpandMode.ServerSide;
                        root.Expanded = true;
                        tvDirectory.Nodes.Add(root);
                    }
                    //Update Library Image
                    imgLibrary.ImageUrl = "~/Images/Libraries/" + LibraryName + ".png";
                }

                //Get content in library based on TreeNodes
                var files = from f in container.AutodeskFiles
                                                 where f.AutodeskFileTreeNodes.Any(n => n.TreeNode.Id == NodeId)
                                                 select f;
                var items = from i in container.Items
                                         where files.Any(f => f.Id == i.AutodeskFile.Id)
                                         select i;
                //Update SearchNames if Needed
                if (updateSearchNames)
                {
                    var search =
                        (from s in container.SearchNames
                         where s.Parameters.Any(p => items.Any(i => i.Id == p.Item.Id))
                         select s.Name).Distinct();
                    Session["Fields"] = search.ToList();
                }
                foreach (var filter in Filters)
                {
                    var tempItems = items;
                    var tempFilter = filter;
                    if (filter.Field == "Any Parameter")
                    {
                        items =
                            from i in
                                tempItems.Where(
                                    i => i.Parameters.Any(p => p.SearchValue.Value.Contains(tempFilter.Value)))
                            select i;
                    }
                    else
                    {
                        if (filter.Op == "Contains")
                        {
                            items =
                                from i in
                                    tempItems.Where(
                                        i =>
                                        i.Parameters.Any(
                                            p =>
                                            p.SearchName.Name == tempFilter.Field &&
                                            p.SearchValue.Value.Contains(tempFilter.Value)))
                                select i;
                        }
                        else
                        {
                            items =
                                from i in
                                    tempItems.Where(
                                        i =>
                                        i.Parameters.Any(
                                            p =>
                                            p.SearchName.Name == tempFilter.Field &&
                                            p.SearchValue.Value == tempFilter.Value))
                                select i;
                        }
                    }
                }
                items = items.OrderBy(i => i.Id);
                //Paging for repeater
                var gridItems = items.Skip(Skip).Take(Take).ToList();
                var finalItems = from g in gridItems
                                 select new
                                            {
                                                g.Id,
                                                g.Name,
                                                g.AutodeskFile, I = g.AutodeskFile.Id,
                                                g.TypeCatalogEntry,
                                                Parameters = g.Parameters.Where(p => p.Featured),
                                            };
                grdItems.VirtualItemCount = items.Count();
                grdItems.DataSource = finalItems;
                grdItems.DataBind();
            }
        }

        #region Download Package

        [WebMethod]
        public static string LoadFile(int familyId)
        {
            var download = CreateDownloadPackage(familyId);
            return download;
        }


        public static string CreateDownloadPackage(int itemId)
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
                using (var container = new BXC_ContentModelEntities())
                {
                    var family = (from c in container.Items
                                  where c.Id == itemId
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
                            sw.WriteLine(family.TypeCatalogLine);
                        }

                        sw.Close();
                        AddtoZip(typeCatalogPath, family.FileName + ".txt", zipFileName, zipFilePath);
                    }
                }
                try
                {
                    using (var container = new BXC_MasterControlEntities())
                    {
                        var download = new Download {DateTime = DateTime.Now, Content_Id = itemId.ToString()};
                        var libName = HttpContext.Current.Session["LibraryName"].ToString();
                        var lib =
                            (from l in container.Libraries where l.Name == libName select l).FirstOrDefault();
                        download.Library = lib;
                        var uname = HttpContext.Current.Session["User"].ToString();
                        download.User = (from u in container.Users where u.UserName == uname select u).FirstOrDefault();
                        container.SaveChanges();
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

        #endregion



        #endregion
    }

    #region Objects

    public class SearchObject
    {
        public SearchObject()
        {
        }

        public SearchObject(string field, string op, string value)
        {
            Field = field;
            Operator = op;
            Value = value;
        }

        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }

    public class Filter
    {
        public Filter()
        {
        }

        public Filter(string field, string op, string value)
        {
            Id = Guid.NewGuid();
            Field = field;
            Op = op;
            Value = value;
        }

        public Guid Id { get; set; }
        public string Field { get; set; }
        public string Op { get; set; }
        public string Value { get; set; }
    }

    #endregion
}