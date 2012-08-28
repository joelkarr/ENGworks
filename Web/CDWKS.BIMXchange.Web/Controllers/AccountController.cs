using System.Web.Mvc;
using System.Web.Security;

using CDWKS.BIMXchange.Web.Controllers.Base;
using CDWKS.BIMXchange.Web.Models;

namespace CDWKS.BIMXchange.Web.Controllers
{
    public class AccountController : BaseController
    {
        #region Actions

        // GET: /Account/LogOn
        public ActionResult LogOn()
        {
            return View();
        }

        // POST: /Account/LogOn
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (Membership.ValidateUser(model.UserName, model.Password))
            {
                Session["IsAuthenticated"] = true;
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // GET: /Account/LogOff
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            return View(model);
        }

        #endregion
    }
}
