using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using System;
using System.Data;
using System.IO;
using System.Runtime.Caching;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;
using UmbracoExamine;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class PageExpiryController : UmbracoAuthorizedController
    {
        private MemoryCache cache = MemoryCache.Default;

        public ActionResult Index()
        {
            var model = cache["PageExpiryViewModel"] as PageExpiryViewModel;

            if (model == null)
            {
                model = new PageExpiryViewModel();
                model.CachedDataAvailable = false;
            }
            return View("~/App_Plugins/EditorTools/Views/PageExpiry/Index.cshtml", model);
        }

        #region Helpers
        public PageExpiryViewModel CreateModel()
        {
            PageExpiryViewModel model = new PageExpiryViewModel();

            model.Expiring.Table = GetExpiringTable(UmbracoContext.Current);
            model.NeverExpires.Table = GetNeverExpiresTable(UmbracoContext.Current);
            model.RecentlyExpired.Table = GetRecentlyExpiredTable();

            return model;
        }

        private static DataTable GetRecentlyExpiredTable()
        {
            var searcher = Examine.ExamineManager.Instance.SearchProviderCollection["InternalSearcher"];
            var criteria = searcher.CreateSearchCriteria(IndexTypes.Content);

            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Edit", typeof(HtmlString));
            table.Columns.Add("Expire Date", typeof(string));

            var recentlyExpiredDateRangeFilter = "+(customExpireDate:[" +
                                                 DateTime.Now.AddDays(-30).Date.ToString("yyyy-MM-dd") + "* TO " +
                                                 DateTime.Now.Date.ToString("yyyy-MM-dd") + "*])";
            var recentlyExpiredQuery =
                criteria.RawQuery("+(__IndexType:content) +(isPublished:false) " + recentlyExpiredDateRangeFilter);
            var recentlyExpiredSearchResults = searcher.Search(recentlyExpiredQuery);

            foreach (var result in recentlyExpiredSearchResults)
            {
                var editURL = new HtmlString(string.Format(
                    "<a target=\"_top\" href=\"/umbraco#/content/content/edit/{0}\">edit</a>",
                    result.Fields["__NodeId"]));

                table.Rows.Add(result.Fields["__NodeId"], result.Fields["nodeName"], editURL,
                    result.Fields["customExpireDate"]);
            }

            return table;
        }

        private static DataTable GetExpiringTable(UmbracoContext umbracoContext)
        {
            var searcher = Examine.ExamineManager.Instance.SearchProviderCollection["InternalSearcher"];
            var criteria = searcher.CreateSearchCriteria(IndexTypes.Content);

            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Published Url", typeof(string));
            table.Columns.Add("Edit", typeof(HtmlString));
            table.Columns.Add("Expire Date", typeof(string));

            var expiringDateRangeFilter = "+(customExpireDate:[" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "* TO " +
                                          DateTime.Now.AddDays(14).Date.ToString("yyyy-MM-dd") + "*])";
            var expiringQuery =
                criteria.RawQuery("+(__IndexType:content) +(isPublished:true) " + expiringDateRangeFilter);
            var expiringSearchResults = searcher.Search(expiringQuery);

            foreach (var result in expiringSearchResults)
            {
                var editURL = new HtmlString(string.Format(
                    "<a target=\"_top\" href=\"/umbraco#/content/content/edit/{0}\">edit</a>",
                    result.Fields["__NodeId"]));

                var contentCacheNode = umbracoContext.ContentCache.GetById(result.Id);
                // Adding <span style=\"font-size:1px\"> into URLs allows them to wrap

                var nodeUrl = contentCacheNode != null
                    ? contentCacheNode.Url.Replace("/", "/<span style=\"font-size:1px\"> </span>")
                    : result.Fields["urlName"];

                table.Rows.Add(result.Fields["__NodeId"], result.Fields["nodeName"], nodeUrl, editURL,
                    result.Fields["customExpireDate"]);
            }

            return table;
        }

        private static DataTable GetNeverExpiresTable(UmbracoContext umbracoContext)
        {
            var searcher = Examine.ExamineManager.Instance.SearchProviderCollection["InternalSearcher"];
            var criteria = searcher.CreateSearchCriteria(IndexTypes.Content);

            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Published Url", typeof(string));
            table.Columns.Add("Edit", typeof(HtmlString));

            var neverExpiresQuery =
                criteria.RawQuery(
                    "+(__IndexType:content) +(isPublished:true) -(customExpireDate:[0001-01-01* TO 3999-12-31*])");
            var neverExpiresSearchResults = searcher.Search(neverExpiresQuery);
            foreach (var result in neverExpiresSearchResults)
            {
                var editURL = new HtmlString(string.Format(
                    "<a target=\"_top\" href=\"/umbraco#/content/content/edit/{0}\">edit</a>",
                    result.Fields["__NodeId"]));

                var contentCacheNode = umbracoContext.ContentCache.GetById(result.Id);

                // Adding <span style=\"font-size:1px\"> into URLs allows them to wrap
                var nodeUrl = contentCacheNode != null
                    ? contentCacheNode.Url.Replace("/", "/<span style=\"font-size:1px\"> </span>")
                    : result.Fields["urlName"];

                table.Rows.Add(result.Id, result.Fields["nodeName"], nodeUrl, editURL);
            }

            return table;
        }

        public UmbracoContext GetUmbracoContext()
        {
            // Ensures the UmbracoContext is available to Async methods that need access.
            var context = new HttpContextWrapper(new HttpContext(new SimpleWorkerRequest("/", string.Empty, new StringWriter())));

            UmbracoContext.EnsureContext(
            context,
            ApplicationContext.Current,
            new WebSecurity(context, ApplicationContext.Current),
            UmbracoConfig.For.UmbracoSettings(),
            UrlProviderResolver.Current.Providers,
            false);

            return UmbracoContext.Current;
        }
        #endregion

        #region Cache Methods
        private void StoreInCache(PageExpiryViewModel model)
        {
            if (cache.Contains("PageExpiryViewModel"))
            {
                cache.Remove("PageExpiryViewModel");
            }

            cache.Add("PageExpiryViewModel", model, DateTime.Now.AddHours(1));
        }

        public ActionResult RefreshCache()
        {
            var model = CreateModel();
            StoreInCache(model);
            return View("~/App_Plugins/EditorTools/Views/PageExpiry/Index.cshtml", model);
        }
        #endregion
    }
}