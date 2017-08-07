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
        ObjectCache cache = MemoryCache.Default;
        public ActionResult Index()
        {
            var Model = new StatsViewModel();
            Model.Users = cache["UsersViewModel"] as UsersViewModel;
            if(Model.Users == null)
            {
                Model.UsersStatsAvailable = false;
                Model.Users.ActiveUsers.Table = new DataTable();
                Model.Users.DisabledUsers.Table = new DataTable();
            }
            else
            {
                Model.UsersStatsAvailable = true;
            }

            Model.Content = cache["ContentViewModel"] as ContentViewModel;
            if (Model.Content == null)
            {
                Model.ContentStatsAvailable = false;
                Model.Content.PublishedContent.Table = new DataTable();
                Model.Content.UnpublishedContent.Table = new DataTable();
            }
            else
            {
                Model.ContentStatsAvailable = true;
            }

            Model.Media = cache["MediaViewModel"] as MediaViewModel;
            if (Model.Media == null)
            {
                Model.MediaStatsAvailable = false;
            }
            else
            {
                Model.MediaStatsAvailable = true;
            }

            Model.PageExpiry = cache["PageExpiryViewModel"] as PageExpiryViewModel;
            if (Model.PageExpiry == null)
            {
                Model.PageExpiryStatsAvailable = false;
            }
            else
            {
                Model.PageExpiryStatsAvailable = true;
            }

            Model.Crawler = cache["InBoundLinkCheckerViewModel"] as InBoundLinkCheckerViewModel;
            if (Model.Crawler == null)
            {
                Model.CrawlerStatsAvailable = false;
            }
            else
            {
                Model.CrawlerStatsAvailable = true;
            }


            return View("~/App_Plugins/EditorTools/Views/Stats/Index.cshtml", Model);
        }
    }
}