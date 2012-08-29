using System;
using System.Web;
using System.Web.Mvc;

using CDWKS.BIMXchange.Web.Constants;

namespace CDWKS.BIMXchange.Web.Providers
{
    public class BIMXAuthorize : AuthorizeAttribute
    {
        #region Overrides

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session != null && httpContext.Session[WebConstants.IsAuthenticated] != null)
            {
                return Convert.ToBoolean(httpContext.Session[WebConstants.IsAuthenticated]);
            }

            return false;
        }

        #endregion
    }
}