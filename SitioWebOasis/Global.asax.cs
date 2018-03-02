using GestorErrores;
using SitioWebOasis.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SitioWebOasis
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }


        protected void Application_EndRequest()
        {
            try
            {
                if (Context.Response.StatusCode == 404)
                {
                    Response.Clear();

                    var rd = new RouteData();
                    rd.DataTokens["area"] = "AreaName"; // In case controller is in another area
                    rd.Values["controller"] = "Errors";
                    rd.Values["action"] = "NotFound";

                    IController c = new ErrorController();
                    c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
                }
            }catch(Exception ex)
            {
                Errores err = new Errores();
                err.SetError(ex, "Application_EndRequest");
            }
        }

    }
}
