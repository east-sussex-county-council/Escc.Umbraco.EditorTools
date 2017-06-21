using System.Web.Mvc;
using System.Web.Routing;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                  name: "Default",
                  url: "umbraco/backoffice/Plugins/{controller}/{action}/{id}",
                  defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
              );
        }
    }
}