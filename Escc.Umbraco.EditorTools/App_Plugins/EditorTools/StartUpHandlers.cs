using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.App_Start;
using Examine;
using System.Web.Routing;
using Umbraco.Core;
using UmbracoExamine;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools
{
    public class StartUpHandlers : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}