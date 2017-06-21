using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class HomeController : UmbracoAuthorizedController
    {
        public ActionResult Index()
        {
            return View("~/App_Plugins/EditorTools/Views/Home/Index.cshtml");
        }
    }
}