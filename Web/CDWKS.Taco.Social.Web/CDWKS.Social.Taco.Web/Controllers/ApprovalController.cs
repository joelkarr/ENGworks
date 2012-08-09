using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Services;
using CDWKS.Social.Taco.Models;
using CDWKS.Social.Taco.UnitOfWork;

namespace CDWKS.Social.Taco.Controllers
{
    public class ApprovalController : Controller
    {
        public ActionResult Index()
        {
            var model = new ApprovalViewModel();
            var context = new EFUnitOfWork();

            if (Session["IsLoggedIn"] != null && Convert.ToBoolean(Session["IsLoggedIn"]))
            {
                try
                {
                    model.IsLoggedIn = true;
                    model.LoginFail = false;
                    model.SocialFeedbackFormList = GetAllSocialFeedback(context);
                }
                finally
                {
                    context.CloseConnection();
                }
            }
            else
            {
                model.IsLoggedIn = false;
            }

            return View(model);
        }

        
        [HttpPost]
        public ActionResult Index(ApprovalViewModel model)
        {
            if (model.Username == ConfigurationManager.AppSettings["Username"] && model.Password == ConfigurationManager.AppSettings["Password"])
            {
                Session["IsLoggedIn"] = true;
                model.IsLoggedIn = true;

                var context = new EFUnitOfWork();

                try
                {
                    model.IsLoggedIn = true;
                    model.LoginFail = false;
                    model.SocialFeedbackFormList = GetAllSocialFeedback(context);
                }
                finally
                {
                    context.CloseConnection();
                }
            }
            else
            {
                model.IsLoggedIn = false;
                model.LoginFail = true;
            }

            return View(model);
        }

        public ActionResult Comments()
        {
            var context = new EFUnitOfWork();
            ApprovalViewModel model = new ApprovalViewModel();

            try
            {
                model.SocialFeedbackFormList = GetAllSocialFeedback(context);
            }
            finally
            {
                context.CloseConnection();
            }

            return View(model);
        }

        private static  List<SocialFeedbackForm> GetAllSocialFeedback(EFUnitOfWork context)
        {
            return context.SocialFeedbackFormRespository.FindAll().OrderByDescending(
                m => m.Timestamp).ToList();
        }
    }
}
