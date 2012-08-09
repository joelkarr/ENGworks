using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;

namespace CDWKS.Service.URLService
{
    public class Global : HttpApplication
    {
        private void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            // Edit the base address of Service1 by replacing the "Service1" string below
            RouteTable.Routes.Add(new ServiceRoute("CDSService", new WebServiceHostFactory(), typeof (CDSURLService)));
        }
    }
}