using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using Examine;
using System;
using System.Runtime.Caching;
using System.Text;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UmbracoExamine;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class CSVExportController : UmbracoAuthorizedController
    {
        private MemoryCache cache = MemoryCache.Default;

        public ActionResult Index()
        {
            var model = new CSVExportViewModel();
            model.CacheDate = cache["CacheDate"] as string;
            return View("~/App_Plugins/EditorTools/Views/CSVExport/Index.cshtml", model);
        }

        #region Helpers
        public void GetFile()
        {
            var CSVString = cache["CSVString"] as StringBuilder;

            if (CSVString == null)
            {
                CSVString = BuildString();
            }

            // When not using an API controller to return a HTTPResonseMessage we must write directly to the HttpContext.Current.Response to return the file.
            string attachment = "attachment; filename=ContentExport.csv";
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.ClearHeaders();
            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.AddHeader("content-disposition", attachment);
            System.Web.HttpContext.Current.Response.ContentType = "text/csv";
            System.Web.HttpContext.Current.Response.AddHeader("Pragma", "public");
            System.Web.HttpContext.Current.Response.Write(CSVString.ToString());
        }

        private StringBuilder BuildString()
        {
            // instantiate a string builder for our csv file
            var csvString = new StringBuilder();

            // append the first row of column names
            csvString.Append("Name,Document Type,Expiry Date,Edit Url,Live Url").Append(Environment.NewLine);

            // get all nodes at the root of the content tree
            var searcher = ExamineManager.Instance.SearchProviderCollection["InternalSearcher"];
            var criteria = searcher.CreateSearchCriteria(IndexTypes.Content);
            var examineQuery = criteria.RawQuery("+__IndexType:content +isDeleted:false");
            var searchResults = searcher.Search(examineQuery);
            foreach (var node in searchResults)
            {
                AddRowForNode(csvString, node);
            }
            StoreInCache(csvString);
            return csvString;
        }

        public void AddRowForNode(StringBuilder sb, SearchResult node)
        {
            var name = node.Fields["nodeName"];
            
            // if the name contains a comma, surround it in "" so the csv doesn't treat each comma as the start of a new column
            if (name.Contains(","))
            {
                name = "\"" + name.Replace("\"", "\"\"") + "\"";
            }

            var docType = node.Fields["__NodeTypeAlias"];
            var expiryDate = node.Fields["customExpireDate"];

            string liveURL;
            if (!String.IsNullOrEmpty(node.Fields["customIsPublished"]) && bool.Parse(node.Fields["customIsPublished"]))
            {
                var contentCacheNode = UmbracoContext.ContentCache.GetById(node.Id);
                liveURL = contentCacheNode?.Url ?? "Parent unpublished";
            }
            else
            {
                liveURL = "Unpublished";
            }
            
            // append to the string builder
            sb.Append(name).Append(",").Append(docType).Append(",").Append(expiryDate).Append(",").Append("/umbraco#/content/content/edit/").Append(node.Id).Append(",").Append(liveURL).Append(Environment.NewLine);
        }

        #endregion

        #region Cache Methods
        private void StoreInCache(StringBuilder CSVString)
        {
            if (cache.Contains("CSVString"))
            {
                cache["CSVString"] = CSVString;
            }
            else
            {
                cache.Add("CSVString", CSVString, System.Web.Caching.Cache.NoAbsoluteExpiration, null);
            }
            if (cache.Contains("CacheDate"))
            {
                cache["CacheDate"] = DateTime.Now.ToString();
            }
            else
            {
                cache.Add("CacheDate", DateTime.Now.ToString(), System.Web.Caching.Cache.NoAbsoluteExpiration, null);
            }
        }

        public ActionResult RefreshCache()
        {
            BuildString();
            GetFile();
            var model = new CSVExportViewModel();
            model.CacheDate = cache["CacheDate"] as string;
            return View("~/App_Plugins/EditorTools/Views/CSVExport/Index.cshtml", model);
        }
        #endregion
    }
}