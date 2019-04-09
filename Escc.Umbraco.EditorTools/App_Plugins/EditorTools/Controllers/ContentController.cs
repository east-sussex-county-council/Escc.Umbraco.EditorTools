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
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class ContentController : UmbracoAuthorizedController
    {
        private MemoryCache _cache = MemoryCache.Default;

        public ActionResult Index()
        {
            var model = AddCachedDataToModel(new ContentViewModel());
            return View("~/App_Plugins/EditorTools/Views/Content/Index.cshtml", model);
        }

        private ContentViewModel AddCachedDataToModel(ContentViewModel model)
        {
            var cachedModel = _cache["ContentViewModel"] as ContentViewModel;

            if (cachedModel == null)
            {
                model.CachedDataAvailable = false;
            }
            else
            {
                model.PublishedContent = cachedModel.PublishedContent;
                model.PublishedPages = cachedModel.PublishedPages;
                model.UnpublishedContent = cachedModel.UnpublishedContent;
                model.UnpublishedPages = cachedModel.UnpublishedPages;
                model.DocumentTypes = cachedModel.DocumentTypes;
                model.TotalPages = cachedModel.TotalPages;
                model.ModalTables = cachedModel.ModalTables;
                model.CachedDataAvailable = true;
            }

            return model;
        }

        [HttpGet]
        public ActionResult RefreshCache(string tab)
        {
            var model = AddContentDataToModel(new ContentViewModel());
            StoreInCache(model);
            model.Tab = tab;
            return View("~/App_Plugins/EditorTools/Views/Content/Index.cshtml", model);
        }

        private ContentViewModel AddContentDataToModel(ContentViewModel model)
        {
            // instantiate the Examine searcher and give it a query
            var searcher = ExamineManager.Instance.SearchProviderCollection["InternalSearcher"];
            var criteria = searcher.CreateSearchCriteria(IndexTypes.Content);
            var examineQuery = criteria.RawQuery("+(__IndexType:content) +__NodeId:[0 TO 999999]");
            var searchResults = searcher.Search(examineQuery);

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

                    // Adding <span style=\"font-size:1px\"> into URLs allows them to wrap
                    var contentCacheNode = UmbracoContext.ContentCache.GetById(Int32.Parse(node.Fields["__NodeId"],CultureInfo.InvariantCulture));
                    model.PublishedContent.Table.Rows.Add(node.Fields["__NodeId"], node.Fields["nodeName"], contentCacheNode != null ? contentCacheNode.Url.Replace("/", "/<span style=\"font-size:1px\"> </span>") : node.Fields["urlName"], editURL);
                }
                else
                {
                    model.UnpublishedPages++;
                    model.UnpublishedContent.Table.Rows.Add(node.Fields["__NodeId"], node.Fields["nodeName"], node.Fields["urlName"], editURL);
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

            model.CachedDataAvailable = true;

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
                    // Adding <span style=\"font-size:1px\"> into URLs allows them to wrap
                    var contentCacheNode = UmbracoContext.ContentCache.GetById(Int32.Parse(result.Fields["__NodeId"],CultureInfo.InvariantCulture));
                    var editURL = new HtmlString(string.Format("<a target=\"_top\" href=\"/umbraco#/content/content/edit/{0}\">edit</a>", result.Fields["__NodeId"]));
                    table.Rows.Add(result.Fields["__NodeId"], result.Fields["nodeName"], contentCacheNode != null ? contentCacheNode.Url.Replace("/", "/<span style=\"font-size:1px\"> </span>") : result.Fields["urlName"], editURL);
                }
            }
            return table;
        }

        [HttpPost]
        public ActionResult GetResults(ContentViewModel model)
        {
            model = AddCachedDataToModel(model);
            // Clean out any reserved characters from the query
            model.Query = CleanString(model.Query);
            // Create a dictionary to store the results and their value
            var ContentResultsDictionary = new Dictionary<SearchResult, int>();

            // instantiate the examine searcher and its criteria type.
            var ContentSearcher = ExamineManager.Instance.SearchProviderCollection["InternalSearcher"];
            var ContentCriteria = ContentSearcher.CreateSearchCriteria(IndexTypes.Content);
            ISearchResults ContentResults;
            if (!string.IsNullOrEmpty(model.Query))
            {
                // Start with a query for the whole phrase
                var ContentPhraseExamineQuery = ContentCriteria.RawQuery(string.Format("+__IndexType:content +urlName:\"{0}\"", model.Query));
                ContentResults = ContentSearcher.Search(ContentPhraseExamineQuery);
            }
            else
            {
                ContentResults = ContentSearcher.Search(ContentCriteria.RawQuery("+(__IndexType:content)"));
            }

            foreach (var result in ContentResults)
            {
                // Add each result to the dictionary and give it an initial value of 1.
                ContentResultsDictionary.Add(result, 1);
                // if our query exactly matched the urlName or the nodeName, Increase the results value to push the result to the top of the list
                if (CleanString(result.Fields["urlName"].ToLower()) == model.Query.ToLower()) ContentResultsDictionary[result] += 5;
                if (CleanString(result.Fields["nodeName"].ToLower()) == model.Query.ToLower()) ContentResultsDictionary[result] += 5;
            }

            // Split the query by its white space
            if (!string.IsNullOrEmpty(model.Query))
            {
                var splitTerm = model.Query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var exampleNotFoundResult = default(KeyValuePair<SearchResult, int>);
                // for each word in the query
                foreach (var term in splitTerm)
                {
                    // do another search for the word
                    ContentCriteria = ContentSearcher.CreateSearchCriteria(IndexTypes.Content);
                    var ContentSplitExamineQuery = ContentCriteria.RawQuery(string.Format("+__IndexType:content +urlName:{0}*", term));
                    var TermContentResults = ContentSearcher.Search(ContentSplitExamineQuery);
                    foreach (var result in TermContentResults)
                    {
                        // if the dictionary doesn't already contain the result, add it and give it an initial value of 1.
                        var existingResult = ContentResultsDictionary.FirstOrDefault(x => x.Key.DocId == result.DocId);
                        if (existingResult.Key == exampleNotFoundResult.Key)
                        {
                            ContentResultsDictionary.Add(result, 1);
                        }
                        else
                        {
                            // if the dicionary did already contain the result, increase its value by 1.
                            ContentResultsDictionary[existingResult.Key] += 1;
                        }
                    }
                }
            }

            // create a new dictionary ordered by the results value
            var OrderedContentResultsDictionary = ContentResultsDictionary.OrderByDescending(x => x.Value);

            // if we have some results, set HasContentResults to true
            if (OrderedContentResultsDictionary.Count() > 0)
            {
                model.HasContentResults = true;
            }

            // instantiate the results datatable
            model.Content.Table = new DataTable();
            model.Content.Table.Columns.Add("ID", typeof(int));
            model.Content.Table.Columns.Add("Name", typeof(string));
            model.Content.Table.Columns.Add("Published Url", typeof(string));
            model.Content.Table.Columns.Add("Edit", typeof(HtmlString));

            // for each results, add a new row for the result to the table.
            foreach (var result in OrderedContentResultsDictionary)
            {
                var content = result.Key;
                var editURL = new HtmlString(string.Format("<a target=\"_top\" href=\"/umbraco#/content/content/edit/{0}\">edit</a>", content.Fields["__NodeId"]));
                model.Content.Table.Rows.Add(content.Fields["__NodeId"], content.Fields["nodeName"], content.Fields["urlName"], editURL);
            }

            return View("~/App_Plugins/EditorTools/Views/Content/Index.cshtml", model);
        }

        private static string CleanString(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var regex = new Regex(@"[^\w\s-]");
                return regex.Replace(text, string.Empty);
            }
            else return string.Empty;
        }

        private void StoreInCache(ContentViewModel model)
        {
            if (_cache.Contains("ContentViewModel"))
            {
                _cache.Remove("ContentViewModel");
            }
            _cache.Add("ContentViewModel", model, DateTime.Now.AddHours(1));
        }
    }
}