using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using CDWKS.Model.EF.MasterControl;

namespace CDWKS.BXC.Web
{
    public partial class Registration : Page
    {

        [WebMethod]
        public static bool IsPromoCodeValid(string promo)
        {
            using (var masterControlEntities = new BXC_MasterControlEntities())
            {
                if (promo == String.Empty)
                {
                    promo = "2e12G15s";
                }
                promo = promo.ToUpper();
                var promotion =
                    (from p in masterControlEntities.Licenses where p.AuthCode == promo select p).FirstOrDefault();
                return promotion != null;
            }
        }

        [WebMethod]
        public static bool RegisterLogin(string userName, string password, string first, string last, string promo,
                                         string company, string phone)
        {
            using (var masterControlEntities = new BXC_MasterControlEntities())
            {
                var alreadyused =
                    (from u in masterControlEntities.Users where u.UserName == userName select u).FirstOrDefault();
                if (alreadyused == null)
                {
                    var newUser = new User {UserName = userName, Password = password};
                    //TODO: encrypt password
                    newUser.ExtendedProperties.Add(CreateProperty("FirstName", first, masterControlEntities));
                    newUser.ExtendedProperties.Add(CreateProperty("LastName", last, masterControlEntities));
                    newUser.ExtendedProperties.Add(CreateProperty("PhoneNumber", phone, masterControlEntities));
                    newUser.ExtendedProperties.Add(CreateProperty("Company", company, masterControlEntities));
                    newUser.ExtendedProperties.Add(CreateProperty("PromoCode", promo, masterControlEntities));
                    newUser.ExtendedProperties.Add(CreateProperty("Discipline", "Mechanical", masterControlEntities));
                    newUser.ExtendedProperties.Add(CreateProperty("CurrentLibrary", "ITT", masterControlEntities));
                    masterControlEntities.Users.AddObject(newUser);
                    masterControlEntities.SaveChanges();
                    HttpContext.Current.Session["User"] = newUser.UserName;
                    HttpContext.Current.Session["Pass"] = newUser.Password;
                    //Check if promo code and add libraries
                    if (promo == String.Empty)
                    {
                        promo = "2e12G15s";
                    }

                    var promotion =
                        (from p in masterControlEntities.Licenses where p.AuthCode == promo select p).FirstOrDefault() ??
                        (from p in masterControlEntities.Licenses where p.AuthCode == "2e12G15s" select p).
                                                                                                                              FirstOrDefault();
                    if (promotion != null)
                        foreach (var lib in promotion.Libraries)
                        {
                            var uLib = new UserLibrary {LibraryId = lib.Id, UserId = newUser.Id};
                            masterControlEntities.UserLibraries.AddObject(uLib);
                            masterControlEntities.SaveChanges();
                        }
                    var currentlibprop = (from p in masterControlEntities.ExtendedProperties
                                                       where
                                                           p.User.Id == newUser.Id &
                                                           p.PropertyName.Name == "CurrentLibrary"
                                                       select p).FirstOrDefault();
                    if (promotion != null)
                    {
                        var currentLib = promotion.Libraries.First().Name;
                        var propValue =
                            (from v in masterControlEntities.PropertyValues where v.Value == currentLib select v).
                                FirstOrDefault();
                        if (propValue != null)
                        {
                            if (currentlibprop != null) currentlibprop.PropertyValue = propValue;
                        }
                        else
                        {
                            var v = new PropertyValue {Value = currentLib};
                            if (currentlibprop != null) currentlibprop.PropertyValue = v;
                        }
                    }
                    masterControlEntities.SaveChanges();
                }
                else
                {
                    return false;
                }
            }

            return true;
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

        protected void BtnBackClick(object sender, EventArgs e)
        {
            Response.Redirect("~/login.aspx");
        }


    }
}