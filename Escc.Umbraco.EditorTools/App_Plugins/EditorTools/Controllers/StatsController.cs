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
            }
            else
            {
                if(Model.Users.ActiveUsers.Table == null || Model.Users.DisabledUsers.Table == null)
                {
                    Model.UsersStatsAvailable = false;
                }
                else
                {
                    Model.UsersStatsAvailable = true;
                }
            }

            Model.Content = cache["ContentViewModel"] as ContentViewModel;
            if (Model.Content == null)
            {
                Model.ContentStatsAvailable = false;
            }
            else
            {
                if (Model.Content.DocumentTypes.Table == null || Model.Content.PublishedContent.Table == null || Model.Content.UnpublishedContent.Table == null)
                {
                    Model.ContentStatsAvailable = false;
                }
                else
                {
                    Model.ContentStatsAvailable = true;
                }

            }

            Model.Media = cache["MediaViewModel"] as MediaViewModel;
            if (Model.Media == null)
            {
                Model.MediaStatsAvailable = false;
            }
            else
            {
                if (Model.Media.Media.Table == null)
                {
                    Model.MediaStatsAvailable = false;
                }
                else
                {
                    Model.MediaStatsAvailable = true;
                }
            }

            Model.PageExpiry = cache["PageExpiryViewModel"] as PageExpiryViewModel;
            if (Model.PageExpiry == null)
            {
                Model.PageExpiryStatsAvailable = false;
            }
            else
            {
                if (Model.PageExpiry.Expiring.Table == null || Model.PageExpiry.NeverExpires.Table == null)
                {
                    Model.PageExpiryStatsAvailable = false;
                }
                else
                {
                    Model.PageExpiryStatsAvailable = true;
                }
            }

            Model.Crawler = cache["InBoundLinkCheckerViewModel"] as InBoundLinkCheckerViewModel;
            if (Model.Crawler == null)
            {
                Model.CrawlerStatsAvailable = false;
            }
            else
            {
                if (Model.Crawler.BrokenLinks.Table == null || Model.Crawler.InBoundLinks.Table == null || Model.Crawler.Domains.Table == null || Model.Crawler.LinksFoundTable.Table == null)
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