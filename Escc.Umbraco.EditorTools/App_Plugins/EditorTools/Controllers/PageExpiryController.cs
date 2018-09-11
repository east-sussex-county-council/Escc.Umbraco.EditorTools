using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using System;
using System.Data;
using System.Globalization;
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
            // create the datatables and their columns
            model.Expiring.Table = new DataTable();
            model.Expiring.Table.Columns.Add("ID", typeof(int));
            model.Expiring.Table.Columns.Add("Name", typeof(string));
            model.Expiring.Table.Columns.Add("Published Url", typeof(string));
            model.Expiring.Table.Columns.Add("Edit", typeof(HtmlString));
            model.Expiring.Table.Columns.Add("Expire Date", typeof(string));

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

                // If the result doesn't contain the expireDate key
                if (!result.Fields.ContainsKey("expireDate"))
                {
                    // If the result is a content node
                    if (result.Fields["__IndexType"] == "content")
                    {
                        var LocalUmbracoContext = GetUmbracoContext();
                        // Get the node from the content service and check it for an expiry date
                        var nodeId = Int32.Parse(result.Fields["__NodeId"], CultureInfo.InvariantCulture);
                        var contentNode = LocalUmbracoContext.Application.Services.ContentService.GetById(nodeId);
                        var contentCacheNode = UmbracoContext.ContentCache.GetById(nodeId);

                        // Adding <span style=\"font-size:1px\"> into URLs allows them to wrap
                        var nodeUrl = contentCacheNode != null ? contentCacheNode.Url.Replace("/", "/<span style=\"font-size:1px\"> </span>") : result.Fields["urlName"];

                        // If it doesn't have one then its a never expire page
                        if (contentNode.ExpireDate == null)
                        {
                            model.NeverExpires.Table.Rows.Add(result.Fields["__NodeId"], result.Fields["nodeName"], nodeUrl, editURL);
                        }
                        else // if it does then its an expiring page
                        {
                            model.Expiring.Table.Rows.Add(result.Fields["__NodeId"], result.Fields["nodeName"], nodeUrl, editURL, contentNode.ExpireDate.Value.ToIsoString());
                            if (contentNode.ExpireDate < DateTime.Now.AddDays(14))
                            {
                                model.TotalExpiresIn14Days++;                              
                            }
                        }
                    }
                }
                // if the result does contain the expireDate key, then it is expiring
                else if (result.Fields.ContainsKey("expireDate"))
                {
                    // Adding <span style=\"font-size:1px\"> into URLs allows them to wrap
                    var nodeId = Int32.Parse(result.Fields["__NodeId"], CultureInfo.InvariantCulture);
                    var contentCacheNode = UmbracoContext.ContentCache.GetById(nodeId);
                    var nodeUrl = contentCacheNode != null ? contentCacheNode.Url.Replace("/", "/<span style=\"font-size:1px\"> </span>") : result.Fields["urlName"];

                    if (result.Fields["expireDate"] == "99991231235959") // DateTime.MaxValue as a proxy for "never expire"
                    {
                        model.NeverExpires.Table.Rows.Add(result.Fields["__NodeId"], result.Fields["nodeName"], nodeUrl, editURL);
                    }
                    else
                    {
                        var expireDateExamine = result.Fields["expireDate"].ToString();
                        var expiryDate = new DateTime(Int32.Parse(expireDateExamine.Substring(0, 4)), Int32.Parse(expireDateExamine.Substring(4, 2)), Int32.Parse(expireDateExamine.Substring(6, 2)), Int32.Parse(expireDateExamine.Substring(8, 2)), Int32.Parse(expireDateExamine.Substring(10, 2)), Int32.Parse(expireDateExamine.Substring(12, 2)));

                        model.Expiring.Table.Rows.Add(result.Fields["__NodeId"], result.Fields["nodeName"], nodeUrl, editURL, expiryDate.ToIsoString());
                    }
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