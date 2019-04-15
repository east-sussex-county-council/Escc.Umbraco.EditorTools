using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using Examine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UmbracoExamine;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class MediaController : UmbracoAuthorizedController
    {
        private MemoryCache _cache = MemoryCache.Default;

        public ActionResult Index()
        {
            var model = AddMediaStatisticsToModelFromCache(new MediaViewModel());
            return View("~/App_Plugins/EditorTools/Views/Media/Index.cshtml", model);
        }

        private MediaViewModel AddMediaStatisticsToModelFromCache(MediaViewModel model)
        {
            var cachedModel = _cache["MediaViewModel"] as MediaViewModel;
            if (cachedModel == null)
            {
                model.StatisticsDataAvailable = false;
            }
            else
            {
                model.StatisticsDataAvailable = true;
                model.TotalMediaFiles = cachedModel.TotalMediaFiles;
                model.TotalFiles = cachedModel.TotalFiles;
                model.TotalFolders = cachedModel.TotalFolders;
                model.TotalImages = cachedModel.TotalImages;
                model.MediaFileTypes = cachedModel.MediaFileTypes;
            }

            return model;
        }

        [HttpGet]
        public ActionResult RefreshCache()
        {
            var model = AddMediaStatisticsToModel(new MediaViewModel());
            StoreInCache(model);
            model.ShowStatistics = true;
            return View("~/App_Plugins/EditorTools/Views/Media/Index.cshtml", model);
        }

        private MediaViewModel AddMediaStatisticsToModel(MediaViewModel model)
        {
            var searcher = ExamineManager.Instance.SearchProviderCollection["InternalSearcher"];
            var criteria = searcher.CreateSearchCriteria(IndexTypes.Media);
            var examineQuery = criteria.RawQuery("+(__IndexType:media)");
            var searchResults = searcher.Search(examineQuery);

            var mediaFileTypeCount = new Dictionary<string, int>();

            foreach (var item in searchResults)
            {
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

            model.StatisticsDataAvailable = true;

            return model;
        }

        [HttpPost]
        public ActionResult GetResults(MediaViewModel model)
        {
            model = AddMediaStatisticsToModelFromCache(model);
            var cleanQuery = CleanString(model.Query);
             
            // Create a dictionary to store the results and their value
            var MediaResultsDictionary = new Dictionary<SearchResult, int>();
            // instantiate the examine search and its criteria type
            var MediaSearcher = ExamineManager.Instance.SearchProviderCollection["InternalSearcher"];
            var MediaCriteria = MediaSearcher.CreateSearchCriteria(IndexTypes.Media);

            // Create a query to get all media nodes and then filter by results that contain the umbracoFile property
            var MediaExamineQuery = MediaCriteria.RawQuery("+(__IndexType:media) -(__NodeTypeAlias:folder)");
            var MediaResults = MediaSearcher.Search(MediaExamineQuery);

            // Check each result for the search terms and assign a value rating that result
            foreach (var result in MediaResults)
            {
                // if the umbracoFile property contains the search  term
                if (CleanString(result.Fields["umbracoFile"]).Contains(cleanQuery.ToLower()))
                {
                    CheckMediaDictionary(MediaResultsDictionary, result);
                }
                // if the nodeName contains the seach term
                if (CleanString(result.Fields["nodeName"]).ToLower().Contains(cleanQuery.ToLower()))
                {
                    CheckMediaDictionary(MediaResultsDictionary, result);
                }
                // if the nodeName exactly equals the seach term
                if (CleanString(result.Fields["nodeName"]).ToLower() == cleanQuery.ToLower())
                {
                    CheckMediaDictionary(MediaResultsDictionary, result);
                }
                // if the search term matches the media id in the umbracoFile property
                if (getMediaID(result) == model.Query)
                {
                    CheckMediaDictionary(MediaResultsDictionary, result);
                }
                var splitMediaQuery = cleanQuery.Split(' ');
                // for each word in the query
                foreach (var term in splitMediaQuery)
                {
                    // if the term is found in the the umbracoFile property
                    // Note that umbracoFile is not present for a media item which has had its file removed
                    if (CleanString(result.Fields["umbracoFile"]?.ToLower()).Contains(term.ToLower()))
                    {
                        CheckMediaDictionary(MediaResultsDictionary, result);
                    }
                    // if the term is found in the nodeName property
                    if (CleanString(result.Fields["nodeName"].ToLower()).Contains(term.ToLower()))
                    {
                        CheckMediaDictionary(MediaResultsDictionary, result);
                    }
                }
            }

            // create a new dictionary ordered by the results value
            var OrderedMediaResultsDictionary = MediaResultsDictionary.OrderByDescending(x => x.Value);

            // if the search returned any results, set HasMediaResults to true
            if (OrderedMediaResultsDictionary.Count() > 0)
            {
                model.HasMediaResults = true;
            }

            // instantiate the media results datatable
            model.Media.Table = new DataTable();
            model.Media.Table.Columns.Add("ID", typeof(string));
            model.Media.Table.Columns.Add("Name", typeof(string));
            model.Media.Table.Columns.Add("Type", typeof(string));
            model.Media.Table.Columns.Add("Date Created", typeof(string));
            model.Media.Table.Columns.Add("Created By", typeof(string));
            model.Media.Table.Columns.Add("Edit", typeof(HtmlString));

            // for each result, add a new row to the table
            foreach (var result in OrderedMediaResultsDictionary)
            {
                var media = result.Key;
                var id = getMediaID(result.Key);

                var edit = new HtmlString(string.Format("<a target=\"_top\" href=\"/umbraco#/media/media/edit/{0}\">edit</a>", media.Fields["__NodeId"]));
                model.Media.Table.Rows.Add(id, 
                    media.Fields["nodeName"], 
                    media.Fields["umbracoExtension"], 
                    ParseLuceneDate(media.Fields["createDate"]).ToString(), 
                    media.Fields["writerName"], 
                    edit);
            }

            return View("~/App_Plugins/EditorTools/Views/Media/Index.cshtml", model);
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

        private static void CheckMediaDictionary(Dictionary<SearchResult, int> MediaResultsDictionary, SearchResult result)
        {
            // Check the media results dictionary and either add a new entry or add to the results value
            if (!MediaResultsDictionary.Keys.Contains(result))
            {
                MediaResultsDictionary.Add(result, 1);
            }
            else
            {
                MediaResultsDictionary[result] += 1;
            }
        }

        private static string getMediaID(SearchResult result)
        {
            // Split the UmbracoFile property to get the media ID 
            // Note that umbracoFile is not present for a media item which has had its file removed
            if (!string.IsNullOrEmpty(result.Fields["umbracoFile"]))
            {
                var umbracoFileName = result.Fields["umbracoFile"];
                var splitFileName = umbracoFileName.Split('/');
                var id = splitFileName[2];
                return id;
            }
            else return string.Empty;
        }

        private static DateTime? ParseLuceneDate(string luceneDateTime)
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
            catch (FormatException)
            {
                return null;
            }
        }

        private void StoreInCache(MediaViewModel model)
        {
            if (_cache.Contains("MediaViewModel"))
            {
                _cache.Remove("MediaViewModel");
            }

            _cache.Add("MediaViewModel", model, DateTime.Now.AddHours(1));
        }

    }
}