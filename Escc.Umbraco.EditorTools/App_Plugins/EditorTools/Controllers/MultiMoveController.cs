using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using Examine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UmbracoExamine;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class MultiMoveController : UmbracoAuthorizedController
    {
        public ActionResult Index(string SuccessMessage = "", string WarningMessage = "", string ErrorMessage = "")
        {
            MultiMoveViewModel model = PrepareIndexViewModel();
            model.SuccessMessage = SuccessMessage;
            model.WarningMessage = WarningMessage;
            model.ErrorMessage = ErrorMessage;

            return View("~/App_Plugins/EditorTools/Views/MultiMove/Index.cshtml", model);
        }

        private static MultiMoveViewModel PrepareIndexViewModel()
        {
            var model = new MultiMoveViewModel();
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

        [HttpPost]
        public ActionResult ConfirmMoveSelected(string[] MoveContent)
        {
            MultiMoveViewModel model = PrepareIndexViewModel();
            var ViewModel = new MultiMoveConfirmSelectedViewModel();

            var FilterSelected = MoveContent.Where(x => x.ToLower().Split(',')[0] != "false");
            foreach (var item in FilterSelected)
            {
                ViewModel.Selected.Add(int.Parse(item.Split(',')[0]), item.Split(',')[1]);
            }
            ViewModel.Content = model.Content;

            if (ViewModel.Selected.Count == 0)
            {
                return Index("", "You did not select any pages.");
            }
            else
            {
                return View("~/App_Plugins/EditorTools/Views/MultiMove/ConfirmSelected.cshtml", ViewModel);
            }
        }

        [HttpPost]
        public ActionResult MoveContent(List<int> Selected, int? ParentID)
        {
            try
            {
                if (ParentID == null)
                {
                    return Index("", "You did not select a parent! Move cancelled.");
                }
                foreach (var item in Selected)
                {
                    var node = Umbraco.UmbracoContext.Application.Services.ContentService.GetById(item);
                    var children = Umbraco.UmbracoContext.Application.Services.ContentService.GetDescendants(node.Id);
                    node.ParentId = (int)ParentID;           
                    Umbraco.UmbracoContext.Application.Services.ContentService.Save(node, 0, false);

                    // Even though the parent ID doesn't change make sure to to update the children and save so their Path properties are valid.
                    foreach (var child in children)
                    {
                        child.ParentId = child.ParentId;
                        Umbraco.UmbracoContext.Application.Services.ContentService.Save(child, 0, false);
                    }
                }
                umbraco.library.RefreshContent();
                ExamineManager.Instance.IndexProviderCollection["InternalIndexer"].RebuildIndex();
                return Index("Move Successful! If you don't notice the change straight away try refreshing the page.");
            }
            catch (Exception ex)
            {
                return Index("", "", string.Format("Error during move. Exception: {0}", ex.Message));
                throw;
            }

        }
    }
}