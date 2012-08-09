using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using CDWKS.Model.EF.MasterControl;

namespace CDWKS.BXC.Web
{
    public partial class Library : Page
    {
// ReSharper disable InconsistentNaming
        protected void Page_Load(object sender, EventArgs e)
// ReSharper restore InconsistentNaming
        {
            if (!IsPostBack)
            {
                LoadLibraries();
            }
        }

        [WebMethod]
        public static bool UpdateLibrary(string libraryName)
        {
            var userName = HttpContext.Current.Session["User"].ToString();
            using (var masterControl = new BXC_MasterControlEntities())
            {
                var user = (from u in masterControl.Users where u.UserName == userName select u).FirstOrDefault();
                if (user != null)
                {
                    var currentlibprop =
                        (from p in masterControl.ExtendedProperties
                         where p.User.Id == user.Id & p.PropertyName.Name == "CurrentLibrary"
                         select p).FirstOrDefault();
                    var propValue =
                        (from v in masterControl.PropertyValues where v.Value == libraryName select v).FirstOrDefault();
                    if (propValue != null)
                    {
                        if (currentlibprop != null) currentlibprop.PropertyValue = propValue;
                    }
                    else
                    {
                        var v = new PropertyValue {Value = libraryName};
                        if (currentlibprop != null) currentlibprop.PropertyValue = v;
                    }
                    masterControl.SaveChanges();
                    HttpContext.Current.Session["UpdateLibrary"] = true;
                }
            }
            return true;
        }

        protected void BtnAddPromoClick(object sender, EventArgs e)
        {
            if (Session["User"] == null) return;
            var userName = Session["User"].ToString();
            using (var masterControl = new BXC_MasterControlEntities())
            {
                var user = (from u in masterControl.Users where u.UserName == userName select u.Id).FirstOrDefault();
                if (user != 0)
                {
                    var promo =
                        (from l in masterControl.Licenses
                         where l.AuthCode == txtEnterPromo.Text.ToUpper()
                         select l.Libraries).FirstOrDefault();
                    if (promo != null)
                    {
                        foreach (var lib in promo)
                        {
                            var alreadyExists =
                                (from l in masterControl.UserLibraries
                                 where l.LibraryId == lib.Id & l.UserId == user
                                 select l).FirstOrDefault();
                            if (alreadyExists == null)
                            {
                                var uLib = new UserLibrary {LibraryId = lib.Id, UserId = user};
                                masterControl.UserLibraries.AddObject(uLib);
                                masterControl.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        litError.Visible = true;
                    }
                }
                var extProp =
                    (from prop in masterControl.PropertyNames where prop.Name == "Codes" select prop.Id).
                        FirstOrDefault();
                if (extProp != 0)
                {
                    var exProp =
                        (from prop in masterControl.ExtendedProperties
                         where prop.User.Id == user && prop.PropertyName.Id == extProp
                         select prop).FirstOrDefault();
                    if (exProp != null)
                    {
                        var allcodes = exProp.PropertyValue.Value;
                        allcodes = allcodes + "," + txtEnterPromo.Text;
                        var v = new PropertyValue {Value = allcodes};
                        exProp.PropertyValue = v;
                    }
                    masterControl.SaveChanges();
                }
            }
            LoadLibraries();
        }

        public void LoadLibraries()
        {
            if (Session["User"] == null) return;
            var userName = Session["User"].ToString();
            using (var masterControl = new BXC_MasterControlEntities())
            {
                var user = (from u in masterControl.Users where u.UserName == userName select u.Id).FirstOrDefault();
                if (user == 0) return;
                var libs = from u in masterControl.UserLibraries
                                          join l in masterControl.Libraries on u.LibraryId equals l.Id
                                          where u.UserId == user
                                          select l.Name;
                ddLibraries.DataSource = libs.ToList();
                ddLibraries.DataBind();
            }
        }
    }
}