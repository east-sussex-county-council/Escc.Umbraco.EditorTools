using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UmbracoExamine;
using Examine;
using System;
using System.Runtime.Caching;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class ContentController : UmbracoAuthorizedController
    {
        ObjectCache cache = MemoryCache.Default;
        public ActionResult Index()
        {
            var model = cache["ContentViewModel"] as ContentViewModel;

            if (model == null)
            {
                model = CreateModel();
                StoreInCache(model);
            }
            return View("~/App_Plugins/EditorTools/Views/Content/Index.cshtml", model);
        }

        #region Helpers
        private ContentViewModel CreateModel()
        {
            var model = new ContentViewModel();
            // instantiate the Examine searcher and give it a query
            var Searcher = Examine.ExamineManager.Instance.SearchProviderCollection["InternalSearcher"];
            var criteria = Searcher.CreateSearchCriteria(IndexTypes.Content);
            var examineQuery = criteria.RawQuery("__NodeId:[0 TO 999999]");
            var searchResults = Searcher.Search(examineQuery);

            model.PublishedContent.Table = new DataTable();
            model.PublishedContent.Table.Columns.Add("ID", typeof(int));
            model.PublishedContent.Table.Columns.Add("Name", typeof(string));
            model.PublishedContent.Table.Columns.Add("Published Url", typeof(string));
            model.PublishedContent.Table.Columns.Add("Edit", typeof(HtmlString));

            model.UnpublishedContent.Table = new DataTable();
            model.UnpublishedContent.Table.Columns.Add("ID", typeof(int));
            model.UnpublishedContent.Table.Columns.Add("Name", typeof(string));
            model.UnpublishedContent.Table.Columns.Add("Published Url", typeof(string));
            model.UnpublishedContent.Table.Columns.Add("Edit", typeof(HtmlString));

            model.DocumentTypes.Table = new DataTable();
            model.DocumentTypes.Table.Columns.Add("Count", typeof(int));
            model.DocumentTypes.Table.Columns.Add("Document Type", typeof(string));
            model.DocumentTypes.Table.Columns.Add("View", typeof(HtmlString));

            var DocumentTypesDictionary = new Dictionary<string, int>();
            foreach (var node in searchResults)
            {
                var key = node.Fields["__NodeTypeAlias"];
                if (node.Fields["__IndexType"] == "content")
                {
                    var editURL = new HtmlString(string.Format("<a target=\"_top\" href=\"/umbraco#/content/content/edit/{0}\">edit</a>", node.Fields["__NodeId"]));
                    model.TotalPages++;
                    if (DocumentTypesDictionary.ContainsKey(key))
                    {
                        DocumentTypesDictionary[key] = DocumentTypesDictionary[key] + 1;
                    }
                    else
                    {
                        DocumentTypesDictionary.Add(key, 1);
                    }
                    if (UmbracoContext.Application.Services.ContentService.HasPublishedVersion(int.Parse(node.Fields["__NodeId"])))
                    {
                        model.PublishedPages++;
                        model.PublishedContent.Table.Rows.Add(node.Fields["__NodeId"], node.Fields["nodeName"], node.Fields["urlName"], editURL);
                    }
                    else
                    {
                        model.UnpublishedPages++;
                        model.UnpublishedContent.Table.Rows.Add(node.Fields["__NodeId"], node.Fields["nodeName"], node.Fields["urlName"], editURL);
                    }
                }
            }

            foreach (var document in DocumentTypesDictionary)
            {
                var buttonString = new HtmlString(string.Format("<button type=\"button\" class=\"btn btn-primary btn-sm\" data-toggle=\"modal\" data-target=\"#{0}\">View</button>", document.Key));
                model.DocumentTypes.Table.Rows.Add(document.Value, document.Key, buttonString);

                var tableModel = new TableModel(document.Key + "Table");

                tableModel.Table = CreateModalTable(searchResults, document.Key);
                model.ModalTables.Add(document.Key, tableModel);
            }

            return model;
        }

        private DataTable CreateModalTable(ISearchResults searchResults, string DocumentType)
        {
            var table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Published Url", typeof(string));
            table.Columns.Add("Edit", typeof(HtmlString));

            foreach (var result in searchResults)
            {
                if (result.Fields["__NodeTypeAlias"] == DocumentType)
                {
                    var editURL = new HtmlString(string.Format("<a target=\"_top\" href=\"/umbraco#/content/content/edit/{0}\">edit</a>", result.Fields["__NodeId"]));
                    table.Rows.Add(result.Fields["__NodeId"], result.Fields["nodeName"], result.Fields["urlName"], editURL);
                }
            }
            return table;
        }

        #endregion

        #region Cache Methods
        private void StoreInCache(ContentViewModel model)
        {
            cache.Add("ContentViewModel", model, System.Web.Caching.Cache.NoAbsoluteExpiration, null);
        }

        public ActionResult RefreshCache()
        {
            var model = CreateModel();
            StoreInCache(model);
            return View("~/App_Plugins/EditorTools/Views/Content/Index.cshtml", model);
        }
        #endregion
    }
}