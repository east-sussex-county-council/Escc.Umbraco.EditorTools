using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using Examine;
using Examine.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
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

        #region Helpers
        [HttpPost]
        public ActionResult GetResults(ExamineSearchViewModel PostModel)
        {
            var model = new ExamineSearchViewModel();
            model.Query = PostModel.Query;
            var cleanQuery = CleanString(PostModel.Query);
            switch (PostModel.SearchType)
            {
                case "Media":
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
                        if (getMediaID(result) == PostModel.Query)
                        {
                            CheckMediaDictionary(MediaResultsDictionary, result);
                        }
                        var splitMediaQuery = cleanQuery.Split(' ');
                        // for each word in the query
                        foreach (var term in splitMediaQuery)
                        {
                            // if the term is found in the the umbracoFile property
                            if (CleanString(result.Fields["umbracoFile"].ToLower()).Contains(term.ToLower()))
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
                    if (OrderedMediaResultsDictionary.Count() > 0) model.HasMediaResults = true;

                    // instantiate the media results datatable
                    model.MediaTable.Table = new DataTable();
                    model.MediaTable.Table.Columns.Add("ID", typeof(string));
                    model.MediaTable.Table.Columns.Add("Name", typeof(string));
                    model.MediaTable.Table.Columns.Add("Type", typeof(string));
                    model.MediaTable.Table.Columns.Add("Edit", typeof(HtmlString));

                    // for each result, add a new row to the table
                    foreach (var result in OrderedMediaResultsDictionary)
                    {
                        var media = result.Key;
                        var id = getMediaID(result.Key);

                        var edit = new HtmlString(string.Format("<a target=\"_top\" href=\"/umbraco#/media/media/edit/{0}\">edit</a>", media.Fields["__NodeId"]));
                        model.MediaTable.Table.Rows.Add(id, media.Fields["nodeName"], media.Fields["umbracoExtension"], edit);
                    }
                    break;

                case "Content":
                    // Clean out any reserved characters from the query
                    PostModel.Query = CleanString(PostModel.Query);
                    // Create a dictionary to store the results and their value
                    var ContentResultsDictionary = new Dictionary<SearchResult, int>();

                    // instantiate the examine searcher and its criteria type.
                    var ContentSearcher = ExamineManager.Instance.SearchProviderCollection["InternalSearcher"];
                    var ContentCriteria = ContentSearcher.CreateSearchCriteria(IndexTypes.Content);
                    ISearchResults ContentResults;
                    if (!string.IsNullOrEmpty(PostModel.Query))
                    {
                        // Start with a query for the whole phrase
                        var ContentPhraseExamineQuery = ContentCriteria.RawQuery(string.Format("+__IndexType:content +urlName:\"{0}\"", PostModel.Query));
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
                        if (CleanString(result.Fields["urlName"].ToLower()) == PostModel.Query.ToLower()) ContentResultsDictionary[result] += 5;
                        if (CleanString(result.Fields["nodeName"].ToLower()) == PostModel.Query.ToLower()) ContentResultsDictionary[result] += 5;
                    }

                    // Split the query by its white space
                    if (!string.IsNullOrEmpty(PostModel.Query))
                    {
                        var splitTerm = PostModel.Query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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
                    if (OrderedContentResultsDictionary.Count() > 0) model.HasContentResults = true;

                    // instantiate the results datatable
                    model.ContentTable.Table = new DataTable();
                    model.ContentTable.Table.Columns.Add("ID", typeof(int));
                    model.ContentTable.Table.Columns.Add("Name", typeof(string));
                    model.ContentTable.Table.Columns.Add("Published Url", typeof(string));
                    model.ContentTable.Table.Columns.Add("Edit", typeof(HtmlString));

                    // for each results, add a new row for the result to the table.
                    foreach (var result in OrderedContentResultsDictionary)
                    {
                        var content = result.Key;
                        var editURL = new HtmlString(string.Format("<a target=\"_top\" href=\"/umbraco#/content/content/edit/{0}\">edit</a>", content.Fields["__NodeId"]));
                        model.ContentTable.Table.Rows.Add(content.Fields["__NodeId"], content.Fields["nodeName"], content.Fields["urlName"], editURL);
                    }
                    break;
            }

            return View("~/App_Plugins/EditorTools/Views/ExamineSearch/Index.cshtml", model);
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

        public string getMediaID(SearchResult result)
        {
            //Split the UmbracoFile property to get the media ID 
            var umbracoFileName = result.Fields["umbracoFile"];
            var splitFileName = umbracoFileName.Split('/');
            var id = splitFileName[2];
            return id;
        }

        public string CleanString(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var regex = new Regex(@"[^\w\s-]");
                return regex.Replace(text, string.Empty);
            }
            else return string.Empty;
        }
        #endregion
    }
}