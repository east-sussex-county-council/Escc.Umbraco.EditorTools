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

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class DocumentTypesController : UmbracoAuthorizedController
    {
        public ActionResult Index()
        {
            var model = new DocumentTypesViewModel();

            // instantiate the Examine searcher and give it a query
            var Searcher = Examine.ExamineManager.Instance.SearchProviderCollection["ExternalSearcher"];
            var criteria = Searcher.CreateSearchCriteria(IndexTypes.Content);
            var examineQuery = criteria.RawQuery("__NodeId:[0 TO 999999]");
            var searchResults = Searcher.Search(examineQuery);

            var DocumentTypesDictionary = new Dictionary<string, int>();
            foreach (var node in searchResults)
            {
                var key = node.Fields["__NodeTypeAlias"];
                if (node.Fields["__IndexType"] == "content")
                {
                    if (DocumentTypesDictionary.ContainsKey(key))
                    {
                        DocumentTypesDictionary[key] = DocumentTypesDictionary[key] + 1;
                    }
                    else
                    {
                        DocumentTypesDictionary.Add(key, 1);
                    }
                }
            }

            model.DocumentTypes.Table = new DataTable();
            model.DocumentTypes.Table.Columns.Add("Count", typeof(int));
            model.DocumentTypes.Table.Columns.Add("Document Type", typeof(string));
            model.DocumentTypes.Table.Columns.Add("View", typeof(HtmlString));

            foreach (var document in DocumentTypesDictionary)
            {
                var buttonString = new HtmlString(string.Format("<button type=\"button\" class=\"btn btn-primary btn-sm\" data-toggle=\"modal\" data-target=\"#{0}\">View</button>", document.Key));
                model.DocumentTypes.Table.Rows.Add(document.Value, document.Key, buttonString);

                var tableModel = new TableModel(document.Key + "Table");

                tableModel.Table = CreateTable(searchResults, document.Key);
                model.ModalTables.Add(document.Key, tableModel);
            }


            return View("~/App_Plugins/EditorTools/Views/DocumentTypes/Index.cshtml", model);
        }

        private DataTable CreateTable(ISearchResults searchResults, string DocumentType)
        {
            var table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Published Url", typeof(string));
            table.Columns.Add("Edit", typeof(HtmlString));

            foreach (var result in searchResults)
            {
                if(result.Fields["__NodeTypeAlias"] == DocumentType)
                {
                    var editURL = new HtmlString(string.Format("<a target=\"_top\" href=\"/umbraco#/content/content/edit/{0}\">edit</a>", result.Fields["__NodeId"]));
                    table.Rows.Add(result.Fields["__NodeId"], result.Fields["nodeName"], result.Fields["urlName"], editURL);
                }
            }

            return table;
        }
    }
}