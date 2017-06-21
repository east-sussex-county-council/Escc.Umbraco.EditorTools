using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using Examine;
using Examine.SearchCriteria;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UmbracoExamine;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class ExamineSearchController : UmbracoAuthorizedController
    {
        public ActionResult Index()
        {
            var model = new ExamineSearchViewModel();

            return View("~/App_Plugins/EditorTools/Views/ExamineSearch/Index.cshtml", model);
        }

        [HttpPost]
        public ActionResult GetResults(ExamineSearchViewModel PostModel)
        {
            var model = new ExamineSearchViewModel();
            model.Query = PostModel.Query;

            switch (PostModel.SearchType)
            {
                case "Media":
                    var MediaSearcher = Examine.ExamineManager.Instance.SearchProviderCollection["InternalSearcher"];
                    var MediaCriteria = MediaSearcher.CreateSearchCriteria(IndexTypes.Content);
                    var MediaExamineQuery = MediaCriteria.RawQuery(string.Format("umbracoFile:/media*{0}*", PostModel.Query));
                    var MediaResults = MediaSearcher.Search(MediaExamineQuery);

                    if (MediaResults.TotalItemCount > 0) model.HasMediaResults = true;

                    model.MediaTable.Table = new DataTable();
                    model.MediaTable.Table.Columns.Add("ID", typeof(string));
                    model.MediaTable.Table.Columns.Add("Name", typeof(string));
                    model.MediaTable.Table.Columns.Add("Type", typeof(string));
                    model.MediaTable.Table.Columns.Add("Edit", typeof(HtmlString));

                    foreach (var media in MediaResults)
                    {
                        var umbracoFileName = media.Fields["umbracoFile"];
                        var splitFileName = umbracoFileName.Split('/');
                        var id = splitFileName[2];

                        var edit = new HtmlString(string.Format("<a target=\"_top\" href=\"/umbraco#/media/media/edit/{0}\">edit</a>", media.Fields["__NodeId"]));
                        model.MediaTable.Table.Rows.Add(id,media.Fields["nodeName"], media.Fields["umbracoExtension"], edit);
                    }              
                    break;
                case "Content":
                    var ContentSearcher = Examine.ExamineManager.Instance.SearchProviderCollection["ExternalSearcher"];
                    var ContentCriteria = ContentSearcher.CreateSearchCriteria(IndexTypes.Content);

                    var ContentExamineQuery = ContentCriteria.RawQuery(string.Format("urlName:{0}", PostModel.Query));
                    var ContentResults = ContentSearcher.Search(ContentExamineQuery);

                    if (ContentResults.TotalItemCount > 0) model.HasContentResults = true;

                    model.ContentTable.Table = new DataTable();
                    model.ContentTable.Table.Columns.Add("ID", typeof(int));
                    model.ContentTable.Table.Columns.Add("Name", typeof(string));
                    model.ContentTable.Table.Columns.Add("Published Url", typeof(string));
                    model.ContentTable.Table.Columns.Add("Edit", typeof(HtmlString));

                    foreach (var content in ContentResults)
                    {
                        var editURL = new HtmlString(string.Format("<a target=\"_top\" href=\"/umbraco#/content/content/edit/{0}\">edit</a>", content.Fields["__NodeId"]));
                        model.ContentTable.Table.Rows.Add(content.Fields["__NodeId"], content.Fields["nodeName"], content.Fields["urlName"], editURL);
                    }
                    break;
            }

            return View("~/App_Plugins/EditorTools/Views/ExamineSearch/Index.cshtml", model);
        }
    }
}