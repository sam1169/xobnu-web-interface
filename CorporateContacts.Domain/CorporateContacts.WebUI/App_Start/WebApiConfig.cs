using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CorporateContacts.WebUI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("Stripe", "Stripe", new { controller = "Stripe" });
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            config.Routes.MapHttpRoute(name: "DefaultApiAction",routeTemplate: "api/{controller}/{action}");
        }
    }
}
