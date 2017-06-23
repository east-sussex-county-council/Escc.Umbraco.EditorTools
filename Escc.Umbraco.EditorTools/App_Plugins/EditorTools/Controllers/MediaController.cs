using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
                model = new MediaViewModel();
                model.CachedDataAvailable = false;
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

            model.Media.Table = new DataTable();
            model.Media.Table.Columns.Add("Name", typeof(string));
            model.Media.Table.Columns.Add("Date Created", typeof(string));
            model.Media.Table.Columns.Add("Created By", typeof(string));
            model.Media.Table.Columns.Add("Edit Url", typeof(HtmlString));

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
                            var editURL = new HtmlString(string.Format("<a target=\"_top\" href=\"/umbraco#/media/media/edit/{0}\">edit</a>", item.Fields["__NodeId"]));
                            model.Media.Table.Rows.Add(item.Fields["__nodeName"], ParseLuceneDate(item.Fields["createDate"]).ToString(), item.Fields["writerName"], editURL);
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

        public DateTime? ParseLuceneDate(string luceneDateTime)
        {
            if (String.IsNullOrEmpty(luceneDateTime) || luceneDateTime.Length < 14) return null;

            try
            {
                return new DateTime(Int32.Parse(luceneDateTime.Substring(0, 4), CultureInfo.InvariantCulture),
                                    Int32.Parse(luceneDateTime.Substring(4, 2), CultureInfo.InvariantCulture),
                                    Int32.Parse(luceneDateTime.Substring(6, 2), CultureInfo.InvariantCulture),
                                    Int32.Parse(luceneDateTime.Substring(8, 2), CultureInfo.InvariantCulture),
                                    Int32.Parse(luceneDateTime.Substring(10, 2), CultureInfo.InvariantCulture),
                                    Int32.Parse(luceneDateTime.Substring(12, 2), CultureInfo.InvariantCulture));

            }
            catch (FormatException ex)
            {
                return null;
            }
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