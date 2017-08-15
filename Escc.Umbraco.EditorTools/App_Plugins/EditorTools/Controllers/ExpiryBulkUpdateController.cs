using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using Examine;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
    public class ExpiryBulkUpdateController : UmbracoAuthorizedController
    {

        public ActionResult Index(string SuccessMessage = "", string WarningMessage = "", string ErrorMessage = "")
        {
            var model = PrepareIndexViewModel();
            model.SuccessMessage = SuccessMessage;
            model.WarningMessage = WarningMessage;
            model.ErrorMessage = ErrorMessage;

            return View("~/App_Plugins/EditorTools/Views/ExpiryBulkUpdate/Index.cshtml", model);
        }

        [HttpPost]
        public ActionResult BrowseSelected(int? Selected)
        {
            if (Selected == null)
            {
                return Index("", "You did not select a section.");
            }

            var SelectedTable = new TableModel("SelectedTable");
            SelectedTable.Table = new DataTable();
            SelectedTable.Table.Columns.Add("ID", typeof(int));
            SelectedTable.Table.Columns.Add("Name", typeof(string));
            SelectedTable.Table.Columns.Add("Expire Date", typeof(DateTime));
            SelectedTable.Table.Columns.Add("Update", typeof(HtmlString));

            var context = GetUmbracoContext();

            var node = context.Application.Services.ContentService.GetById((int)Selected);

            SelectedTable.Table.Rows.Add(node.Id, node.Name, node.ExpireDate, new HtmlString(string.Format("<div class=\"checkbox\"><label><input type = \"checkbox\" value=\"{0}\" name=\"ToUpdate\"></label></div>", node.Id)));
            var children = context.Application.Services.ContentService.GetDescendants(node.Id);

            foreach (var child in children)
            {
                SelectedTable.Table.Rows.Add(child.Id, child.Name, child.ExpireDate, new HtmlString(string.Format("<div class=\"checkbox\"><label><input type = \"checkbox\" value=\"{0}\" name=\"ToUpdate\"></label></div>", child.Id)));
            }
            return View("~/App_Plugins/EditorTools/Views/ExpiryBulkUpdate/BrowseSelected.cshtml", SelectedTable);
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

        [HttpPost]
        public ActionResult UpdateSelected(string[] ToUpdate, string[] ExpireDate)
        {
            try
            {
         

                if (ToUpdate == null)
                {
                    return Index("", "You did not select any pages. Update cancelled!");
                }
                if (ExpireDate[0] == "")
                {
                    return Index("", "You did not select an expire date. Update cancelled!");
                }
                var UnpublishAt = ExpireDate[0].Replace("PM", "").Replace("AM", "");


                foreach (var item in ToUpdate)
                {
                    var node = Umbraco.UmbracoContext.Application.Services.ContentService.GetById(int.Parse(item));
                    node.ExpireDate = DateTime.Parse(UnpublishAt);
                    Umbraco.UmbracoContext.Application.Services.ContentService.Save(node, 0, false);
                }
                umbraco.library.RefreshContent();
                return Index("Update Successful! If you don't notice the change straight away try refreshing the page.");
            }
            catch (Exception ex)
            {
                return Index("", "", string.Format("Error during update. Exception: {0}", ex.Message));
                throw;
            }
        }



        private static ExpiryBulkUpdateViewModel PrepareIndexViewModel()
        {
            var model = new ExpiryBulkUpdateViewModel();
            // Create a dictionary to store the results and their value
            var ContentResultsDictionary = new Dictionary<SearchResult, int>();

            // instantiate the examine searcher and its criteria type.
            var ContentSearcher = Examine.ExamineManager.Instance.SearchProviderCollection["InternalSearcher"];
            var ContentCriteria = ContentSearcher.CreateSearchCriteria(IndexTypes.Content);

            // Start with a query for the whole phrase
            var ContentPhraseExamineQuery = ContentCriteria.RawQuery(string.Format("__IndexType:content"));
            var ContentResults = ContentSearcher.Search(ContentPhraseExamineQuery);


            var ContentLevel = new Dictionary<int, int>();
            foreach (var content in ContentResults)
            {
                var path = content.Fields["__Path"].Split(',');
                var contentModel = new MultiMoveContentModel(int.Parse(content.Fields["__NodeId"]), content.Fields["nodeName"], int.Parse(content.Fields["parentID"]), path.Count() - 1);

                string description;
                try
                {
                    description = content.Fields["pageDescription"];
                }
                catch (Exception)
                {
                    description = "No Description.";
                }
                contentModel.Description = description;
                model.Content.Add(contentModel);

                ContentLevel.Add(int.Parse(content.Fields["__NodeId"]), path.Count() - 1);
            }

            foreach (var item in ContentLevel)
            {
                var content = model.Content.SingleOrDefault(x => x.ID == item.Key);
                if (content != null)
                {
                    var parent = model.Content.SingleOrDefault(x => x.ID == content.ParentID);
                    if (parent != null)
                    {
                        parent.Children.Add(content.ID);
                    }
                }
            }

            model.ContentLevel = ContentLevel.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            return model;
        }

    }
}