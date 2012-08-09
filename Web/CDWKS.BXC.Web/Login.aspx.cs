using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using CDWKS.Model.EF.MasterControl;


namespace CDWKS.BXC.Web
{
    public partial class Login : Page
    {


        //Login Click
        protected void BtnLoginClick(object sender, EventArgs e)
        {
            if (UserLogin(txtEmail.Text, txtPass.Text))
            {
            }
            else
            {
                //show literal for user or password not found
                litNotFound.Visible = true;
            }
        }

        [WebMethod]
        public static bool UserLogin(string userName, string password)
        {
            using (var masterControl = new BXC_MasterControlEntities())
            {
                var user = (from u in masterControl.Users where u.UserName == userName select u).FirstOrDefault();
                if (user != null && user.Id != 0)
                {
                    var pass =
                        (from u in masterControl.Users where u.UserName == userName select u.Password).FirstOrDefault();
                    if (password == pass)
                    {
                        if (HttpContext.Current.Session["Alias"] != null)
                        {
                            user.ExtendedProperties.Add(CreateProperty("Alias",
                                                                       HttpContext.Current.Session["Alias"].ToString(),
                                                                       masterControl));
                            masterControl.SaveChanges();
                        }
                        HttpContext.Current.Session["User"] = userName;
                        HttpContext.Current.Session["Pass"] = password;
                        return true;
                    }
                }
            }
            return false;
        }

        public static ExtendedProperty CreateProperty(string defName, string value, BXC_MasterControlEntities entity)
        {
            var prop = new ExtendedProperty();
            var name = (from n in entity.PropertyNames where n.Name == defName select n).FirstOrDefault();
            if (name != null)
            {
                prop.PropertyName = name;
            }
            else
            {
                var n = new PropertyName {Name = defName};
                prop.PropertyName = n;
            }
            var propValue = (from v in entity.PropertyValues where v.Value == value select v).FirstOrDefault();
            if (propValue != null)
            {
                prop.PropertyValue = propValue;
            }
            else
            {
                var v = new PropertyValue {Value = value};
                prop.PropertyValue = v;
            }
            return prop;
        }
    }
}