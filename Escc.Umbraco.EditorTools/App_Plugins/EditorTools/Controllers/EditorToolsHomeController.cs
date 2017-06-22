using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class EditorToolsHomeController : UmbracoAuthorizedController
    {
        public ActionResult Index()
        {
            return View("~/App_Plugins/EditorTools/Views/EditorToolsHome/Index.cshtml");
        }

        public ActionResult About()
        {
            return View("~/App_Plugins/EditorTools/Views/EditorToolsHome/About.cshtml");
        }

        public ActionResult Help()
        {
            return View("~/App_Plugins/EditorTools/Views/EditorToolsHome/Help.cshtml");
        }
    }
}