using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SitioWebOasis
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //  Redireccionamiento - DOCENTES
            routes.MapRoute(
                name: "Docentes",
                url: "Docentes/{action}/{strCodNivel}/{strCodAsignatura}/{strCodParalelo}/{strParcialActivo}/{dtaEvAcumulativa}",
                defaults: new
                {
                    controller = "Docentes",
                    action = "Index",
                    strCodNivel = UrlParameter.Optional,
                    strCodAsignatura = UrlParameter.Optional,
                    strCodParalelo = UrlParameter.Optional,
                    strParcialActivo = UrlParameter.Optional,
                    dtaEvAcumulativa = UrlParameter.Optional
                }
            );

            //  Redireccionamiento - DEFAULT
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );
        }
    }
}
