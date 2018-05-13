using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyBookList
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "BO",
                url: "BackOffice/{action}/{id}",
                defaults: new { controller = "BackOffice", action = "Home", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "Activation",
               url: "Account/Activation/{id}/{token}",
               defaults: new { controller = "Account", action = "AtivationComplete" }
           );
            routes.MapRoute(
               name: "Password",
               url: "Account/Password/{id}/{token}",
               defaults: new { controller = "Account", action = "Password" }
           );
        }
    }
}
