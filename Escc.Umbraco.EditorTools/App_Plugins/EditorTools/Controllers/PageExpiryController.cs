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
        ObjectCache cache = MemoryCache.Default;
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
            // create the datatables and their columns
            model.Expiring.Table = new DataTable();
            model.Expiring.Table.Columns.Add("ID", typeof(int));
            model.Expiring.Table.Columns.Add("Name", typeof(string));
            model.Expiring.Table.Columns.Add("Published Url", typeof(string));
            model.Expiring.Table.Columns.Add("Edit", typeof(HtmlString));
            model.Expiring.Table.Columns.Add("Expire Date", typeof(DateTime));

            model.NeverExpires.Table = new DataTable();
            model.NeverExpires.Table.Columns.Add("ID", typeof(int));
            model.NeverExpires.Table.Columns.Add("Name", typeof(string));
            model.NeverExpires.Table.Columns.Add("Published Url", typeof(string));
            model.NeverExpires.Table.Columns.Add("Edit", typeof(HtmlString));

            // instantiate the Examine searcher and give it a query
            var Searcher = Examine.ExamineManager.Instance.SearchProviderCollection["ExternalSearcher"];
            var criteria = Searcher.CreateSearchCriteria(IndexTypes.Content);
            var examineQuery = criteria.RawQuery("__NodeId:[0 TO 999999]");
            var searchResults = Searcher.Search(examineQuery);

            foreach (var result in searchResults)
            {
                var editURL = new HtmlString(string.Format("<a target=\"_top\" href=\"/umbraco#/content/content/edit/{0}\">edit</a>", result.Fields["__NodeId"]));

                // If the result doesn't contain the unpublishAt key
                if (!result.Fields.ContainsKey("unpublishAt"))
                {
                    // If the result is a content node
                    if (result.Fields["__IndexType"] == "content")
                    {
                        var LocalUmbracoContext = GetUmbracoContext();
                        // Get the node from the conent service and check it for an expiry date
                        var contentNode = LocalUmbracoContext.Application.Services.ContentService.GetById(int.Parse(result.Fields["__NodeId"]));

                        // If it doesn't have one then its a never expire page
                        if (contentNode.ExpireDate == null)
                        {
                            model.NeverExpires.Table.Rows.Add(result.Fields["__NodeId"], result.Fields["nodeName"], result.Fields["urlName"], editURL);
                        }
                        else // if it does then its an expiring page
                        {
                            model.Expiring.Table.Rows.Add(result.Fields["__NodeId"], result.Fields["nodeName"], result.Fields["urlName"], editURL, contentNode.ExpireDate);
                            if (contentNode.ExpireDate < DateTime.Now.AddDays(14))
                            {
                                model.TotalExpiresIn14Days++;                              
                            }
                        }
                    }
                }
                // if the result does contain the unpublishAt key, then it is expiring
                else if (result.Fields.ContainsKey("unpublishAt"))
                {
                    model.Expiring.Table.Rows.Add(result.Fields["__NodeId"], result.Fields["nodeName"], result.Fields["urlName"], editURL, result.Fields["unpublishAt"]);
                }
            }
            model.TotalExpiring = model.Expiring.Table.Rows.Count;
            model.TotalNeverExpires = model.NeverExpires.Table.Rows.Count;
        
            return model;
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
                cache["PageExpiryViewModel"] = model;
            }
            else
            {
                cache.Add("PageExpiryViewModel", model, System.Web.Caching.Cache.NoAbsoluteExpiration, null);
            }
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