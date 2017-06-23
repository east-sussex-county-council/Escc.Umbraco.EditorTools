using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UmbracoExamine;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class MediaController : UmbracoAuthorizedController
    {
        ObjectCache cache = MemoryCache.Default;
        public ActionResult Index()
        {
            var model = cache["MediaViewModel"] as MediaViewModel;
            if (model == null)
            {
                // instantiate the view model
                model = new MediaViewModel();
                // populate the view models variables
                model = CreateModel();
                StoreInCache(model);
            }
            return View("~/App_Plugins/EditorTools/Views/Media/Index.cshtml", model);
        }
        #region Helpers

        public MediaViewModel CreateModel()
        {
            var model = new MediaViewModel();

            var Searcher = Examine.ExamineManager.Instance.SearchProviderCollection["InternalSearcher"];
            var criteria = Searcher.CreateSearchCriteria(IndexTypes.Content);
            var examineQuery = criteria.RawQuery("__NodeId:[0 TO 999999]");
            var searchResults = Searcher.Search(examineQuery);

            var mediaFileTypeCount = new Dictionary<string, int>();

            foreach (var item in searchResults)
            {
                switch (item.Fields["__IndexType"])
                {
                    case "media":
                        if (mediaFileTypeCount.Keys.Contains(item.Fields["nodeTypeAlias"]))
                        {
                            mediaFileTypeCount[item.Fields["nodeTypeAlias"]] += 1;
                        }
                        else
                        {
                            mediaFileTypeCount.Add(item.Fields["nodeTypeAlias"], 1);
                        }

                        if (item.Fields.Keys.Contains("umbracoExtension"))
                        {
                            model.TotalMediaFiles++;
                            if (mediaFileTypeCount.Keys.Contains(item.Fields["umbracoExtension"]))
                            {
                                mediaFileTypeCount[item.Fields["umbracoExtension"]] += 1;
                            }
                            else
                            {
                                mediaFileTypeCount.Add(item.Fields["umbracoExtension"], 1);
                            }
                        }
                        break;
                }
            }

            model.TotalFiles = mediaFileTypeCount["File"];
            model.TotalImages = mediaFileTypeCount["Image"];
            model.TotalFolders = mediaFileTypeCount["Folder"];

            mediaFileTypeCount.Remove("File");
            mediaFileTypeCount.Remove("Image");
            mediaFileTypeCount.Remove("Folder");

            model.MediaFileTypes.Table = new DataTable();
            model.MediaFileTypes.Table.Columns.Add("Type", typeof(string));
            model.MediaFileTypes.Table.Columns.Add("Count", typeof(int));        

            foreach (var item in mediaFileTypeCount)
            {
                model.MediaFileTypes.Table.Rows.Add(item.Key, item.Value);
            }

            return model;
        }

        #endregion

        #region Cache Methods
        private void StoreInCache(MediaViewModel model)
        {
            cache.Add("MediaViewModel", model, System.Web.Caching.Cache.NoAbsoluteExpiration, null);
        }

        public ActionResult RefreshCache()
        {
            // instantiate the view model
            var model = CreateModel();
            // populate the view models variables
            StoreInCache(model);
            return View("~/App_Plugins/EditorTools/Views/Media/Index.cshtml", model);
        }
        #endregion
    }
}