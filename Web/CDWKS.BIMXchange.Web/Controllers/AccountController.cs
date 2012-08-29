using System.Web.Mvc;
using System.Web.Security;

using CDWKS.BIMXchange.Web.Constants;
using CDWKS.BIMXchange.Web.Controllers.Base;
using CDWKS.BIMXchange.Web.Models;
using CDWKS.Model.EF.BIMXchange;
using CDWKS.Business.AccountManager;

namespace CDWKS.BIMXchange.Web.Controllers
{
    public partial class AccountController : BaseController
    {
        #region Actions

        // GET: /Account/LogOn
        public virtual ActionResult LogOn()
        {
            return View();
        }

        // POST: /Account/LogOn
        [HttpPost]
        public virtual ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (Membership.ValidateUser(model.UserName, model.Password))
            {
                Session[WebConstants.IsAuthenticated] = true;
                return RedirectToAction(MVC.Home.ActionNames.Index, MVC.Home.Name);
            }

            return View(model);
        }

        // GET: /Account/LogOff
        public virtual ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction(MVC.Home.ActionNames.Index, MVC.Home.Name);
        }

        // GET: /Account/Register

        public virtual ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public virtual ActionResult Register(User model)
        {
            using (var manager = new UserManager())
            {
                //manager.InsertUser(new User());
            }

            return View(model);
        }

        #endregion
    }
}
