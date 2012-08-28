using System;
using System.Web;
using System.Web.Mvc;

namespace CDWKS.BIMXchange.Web.Providers
{
    public class BIMXAuthorize : AuthorizeAttribute
    {
        #region Overrides

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session != null && httpContext.Session["IsAuthenticated"] != null)
            {
                return Convert.ToBoolean(httpContext.Session["IsAuthenticated"]);
            }

            return false;
        }

        #endregion
    }
}