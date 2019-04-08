using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class StatsController : UmbracoAuthorizedController
    {
        private MemoryCache cache = MemoryCache.Default;

        public ActionResult Index()
        {
            var Model = new StatsViewModel();

            Model.Crawler = cache["InBoundLinkCheckerViewModel"] as InBoundLinkCheckerViewModel;
            if (Model.Crawler == null)
            {
                Model.CrawlerStatsAvailable = false;
            }
            else
            {
                if (Model.Crawler.BrokenLinks.Table == null || Model.Crawler.IndexedLinks.Table == null || Model.Crawler.Domains.Table == null || Model.Crawler.LinksFoundTable.Table == null)
                {
                    Model.CrawlerStatsAvailable = false;
                }
                else
                {
                    Model.CrawlerStatsAvailable = true;
                }
            }


            return View("~/App_Plugins/EditorTools/Views/Stats/Index.cshtml", Model);
        }
    }
}