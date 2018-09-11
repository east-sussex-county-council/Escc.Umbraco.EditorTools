using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
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
    public class InBoundLinkCheckerController : UmbracoAuthorizedController
    {
        private MemoryCache cache = MemoryCache.Default;

        #region ActionResults
        public ActionResult Index()
        {
            // get the view model from the cache
            var model = cache["InBoundLinkCheckerViewModel"] as InBoundLinkCheckerViewModel;

            if (model == null)
            {
                // if no model is in the cache then create a new one
                model = new InBoundLinkCheckerViewModel();
                model.CachedDataAvailable = false;
                model.DataBeingGenerated = false;
                StoreModelInCache(model);
            }
            else if (model.DataBeingGenerated)
            {
                // if a crawl is in progress, add a header to refresh the page periodically to update the views progress bar.
                this.HttpContext.Response.AddHeader("refresh", "5; url=" + Url.Action("Index"));
            }

            // if a crawl has been done and cached data is available, then prepare the viewmodel.
            if (model.CachedDataAvailable)
            {
                model = PrepareViewModel(model);
            }



            return View("~/App_Plugins/EditorTools/Views/InBoundLinkChecker/Index.cshtml", model);
        }

        public ActionResult SearchForInBoundLinks(InBoundLinkCheckerViewModel PostModel)
        {
            // If nothing has been entered for the query, just return the index page.
            if (PostModel.Query == null || PostModel.Query == "")
            {
                Index();
            }

            // Get the current ViewModel from the cache
            var model = cache["InBoundLinkCheckerViewModel"] as InBoundLinkCheckerViewModel;
            // Remove any trailing / characters from the query
            model.Query = PostModel.Query.TrimEnd('/');
            model.HasInBoundLinks = false;
            // Get the ValidatedPages from the cache
            var results = cache["ResultsDictionary"] as Dictionary<string, ContentModel>;

            // instantiate the datatable to store the results of the query
            model.InBoundLinks.Table = new DataTable();
            model.InBoundLinks.Table.Columns.Add("Name", typeof(string));
            model.InBoundLinks.Table.Columns.Add("Url", typeof(string));
            model.InBoundLinks.Table.Columns.Add("Edit", typeof(HtmlString));

            // Create a list to store the results of the query
            var PagesWithLink = new List<ContentModel>();
            // for each validated page
            foreach (var node in results)
            {
                // for each link found on that page
                foreach (var link in node.Value.LinksOnNode)
                {
                    // if the link matches the query
                    if (model.Query == link.TrimEnd('/'))
                    {
                        // if that page has not already been added to the list
                        if (!PagesWithLink.Contains(node.Value))
                        {
                            // add the page to list
                            PagesWithLink.Add(node.Value);
                        }
                    }
                    // purely for ESCC, some east sussex pages have new instead of www.
                    else if (model.Query.Replace("www", "new") == link.TrimEnd('/'))
                    {
                        if (!PagesWithLink.Contains(node.Value))
                        {
                            PagesWithLink.Add(node.Value);
                        }
                    }
                }

            }

            // If some results have been found
            if (PagesWithLink.Count > 0)
            {
                // set the boolean to let the view know there are results
                model.HasInBoundLinks = true;
                // for each result found
                foreach (var page in PagesWithLink)
                {
                    // create its edit url and add the page to the datatable
                    var editURL = new HtmlString(string.Format("<a target=\"_top\" href=\"/umbraco#/content/content/edit/{0}\">edit</a>", page.NodeID));
                    model.InBoundLinks.Table.Rows.Add(page.NodeName, page.URL, editURL);
                }
            }
            return View("~/App_Plugins/EditorTools/Views/InBoundLinkChecker/Index.cshtml", model);
        }

        public async Task<ActionResult> StartCrawl(InBoundLinkCheckerViewModel model)
        {
            var Searcher = Examine.ExamineManager.Instance.SearchProviderCollection["ExternalSearcher"];
            var Criteria = Searcher.CreateSearchCriteria(IndexTypes.Content);

            // Create a query to get all nodes of the index type content
            var ExamineQuery = Criteria.RawQuery(string.Format("__IndexType:content"));
            var Results = Searcher.Search(ExamineQuery);


            // Ensure that the collections are new and empty before crawling.
            var ResultsDictionary = new Dictionary<string, ContentModel>();
            var LinksFound = new List<string>();
            var BrokenLinks = new List<BrokenPageModel>();
            var Domains = new List<string>();

            StoreResultsInCache(ResultsDictionary, LinksFound, BrokenLinks, Domains);

            model.TotalToCrawl = Results.Count();
            model.CrawledLinks = 0;
            model.DataBeingGenerated = true;
            StoreModelInCache(model);

            // run async so as not to present the user with a long loading screen and to return the index view.
            Task.Run(() => ManageInternalCrawl(model, Results.ToList()));

            this.HttpContext.Response.AddHeader("refresh", "5; url=" + Url.Action("Index"));
            return View("~/App_Plugins/EditorTools/Views/InBoundLinkChecker/Index.cshtml", model);
        }
        #endregion

        #region Helpers
        public InBoundLinkCheckerViewModel PrepareViewModel(InBoundLinkCheckerViewModel ViewModel)
        {
            // Gather data from the cache
            var ResultsDictionary = cache["ResultsDictionary"] as Dictionary<string, ContentModel>;
            var LinksFound = cache["LinksFound"] as List<string>;
            var BrokenLinks = cache["BrokenLinks"] as List<BrokenPageModel>;
            var Domains = cache["Domains"] as List<string>;

            // Instantiate the view models DataTables
            ViewModel.IndexedLinks.Table = new DataTable();
            ViewModel.IndexedLinks.Table.Columns.Add("Name", typeof(string));
            ViewModel.IndexedLinks.Table.Columns.Add("Published Url", typeof(string));

            ViewModel.BrokenLinks.Table = new DataTable();
            ViewModel.BrokenLinks.Table.Columns.Add("URL", typeof(string));
            ViewModel.BrokenLinks.Table.Columns.Add("Found On", typeof(string));
            ViewModel.BrokenLinks.Table.Columns.Add("Exception", typeof(string));

            ViewModel.LinksFoundTable.Table = new DataTable();
            ViewModel.LinksFoundTable.Table.Columns.Add("Link", typeof(string));

            ViewModel.Domains.Table = new DataTable();
            ViewModel.Domains.Table.Columns.Add("Domain", typeof(string));

            // Populate the DataTables using the cached data
            foreach (var item in ResultsDictionary)
            {
                ViewModel.IndexedLinks.Table.Rows.Add(item.Value.NodeName, item.Value.URL);
            }
            foreach (var item in LinksFound)
            {
                ViewModel.LinksFoundTable.Table.Rows.Add(item);
            }
            foreach (var item in BrokenLinks)
            {
                ViewModel.BrokenLinks.Table.Rows.Add(item.URL, item.FoundOn, item.Exception);
            }
            foreach (var item in Domains)
            {
                ViewModel.Domains.Table.Rows.Add(item);
            }

            ViewModel.TotalBrokenLinks = ViewModel.BrokenLinks.Table.Rows.Count;
            ViewModel.TotalDomainsFound = ViewModel.Domains.Table.Rows.Count;
            ViewModel.TotalUniqueLinks = ViewModel.LinksFoundTable.Table.Rows.Count;
            ViewModel.TotalVerified = ViewModel.IndexedLinks.Table.Rows.Count;

            StoreModelInCache(ViewModel);
            return ViewModel;
        }

        public async Task ManageInternalCrawl(InBoundLinkCheckerViewModel model, List<Examine.SearchResult> PublishedPages)
        {
            // Instantiate Crawler Variables
            var LinksAvailableToCrawl = true;
            var TaskCount = 0;
            var TaskID = 0;
            var TaskStatus = new Dictionary<int, string>();
            var TaskList = new Dictionary<int, Task<CrawlerModel>>();

            // Keep going while there are still umbraco pages to crawl
            while (LinksAvailableToCrawl)
            {
                try
                {
                    // While there are less than 8 async tasks running and at least 1 page to crawl.
                    while (TaskCount < 8 && PublishedPages.Count > 0)
                    {
                        TaskCount++;
                        TaskID++;
                        TaskList.Add(TaskID, Task.Run(() => ProcessPage(model, PublishedPages.Take(1).ToList())));
                        PublishedPages.RemoveRange(0, 1);
                        TaskStatus.Add(TaskID, "Started");
                    }
                    // If there are no pages left to crawl after assigning tasks, set LinksAvailableToCrawl to false to end the while loop after this iteration
                    if (PublishedPages.Count() <= 0 && TaskCount == 0) LinksAvailableToCrawl = false;

                    // Instantiate a List to store the results of the aysnc tasks.
                    var ResultsModelList = new List<CrawlerModel>();

                    // Foreach task in the list, if one is completed, gather its result and log its status as completed.
                    foreach (var Task in TaskList)
                    {
                        if (Task.Value.IsCompleted)
                        {
                            ResultsModelList.Add(Task.Value.Result);
                            TaskStatus[Task.Key] = "Completed";
                        }
                    }

                    // Create a tempory list to log which task keys should be removed.
                    var KeysToRemove = new List<int>();
                    // Foreach task in the status list, if it is logged as completed, remove the task from the task list and add its key to the keystoremove list.
                    foreach (var Task in TaskStatus)
                    {
                        if (Task.Value == "Completed")
                        {
                            TaskList.Remove(Task.Key);
                            TaskCount--;
                            KeysToRemove.Add(Task.Key);
                        }
                    }
                    foreach (var Key in KeysToRemove)
                    {
                        TaskStatus.Remove(Key);
                    }

                    // Instantiate the Collections to store results in.
                    var ResultsDictionary = cache["ResultsDictionary"] as Dictionary<string, ContentModel>;
                    if (ResultsDictionary == null)
                    {
                        ResultsDictionary = new Dictionary<string, ContentModel>();
                    }

                    var BrokenLinks = cache["BrokenLinks"] as List<BrokenPageModel>;
                    if (BrokenLinks == null)
                    {
                        BrokenLinks = new List<BrokenPageModel>();
                    }
                    var LinksFound = cache["LinksFound"] as List<string>;
                    if (LinksFound == null)
                    {
                        LinksFound = new List<string>();
                    }
                    var Domains = cache["Domains"] as List<string>;
                    if (Domains == null)
                    {
                        Domains = new List<string>();
                    }

                    // For each result model in the resultmodellist, if it is not null, process the models results into the results collections.
                    foreach (var ResultModel in ResultsModelList)
                    {
                        if (ResultModel != null)
                        {
                            model.CrawledLinks += ResultModel.CrawledLinks;
                            foreach (var item in ResultModel.BrokenLinks)
                            {
                                if (!BrokenLinks.Contains(item))
                                {
                                    BrokenLinks.Add(item);
                                }
                            }
                            foreach (var item in ResultModel.ResultsDictionary)
                            {
                                if (!ResultsDictionary.Keys.Contains(item.Key))
                                {
                                    ResultsDictionary.Add(item.Key, item.Value);
                                }
                            }
                            foreach (var item in ResultModel.LinksFound)
                            {
                                if (!LinksFound.Contains(item))
                                {
                                    LinksFound.Add(item);
                                }
                            }
                            foreach (var item in ResultModel.Domains)
                            {
                                if (!Domains.Contains(item))
                                {
                                    Domains.Add(item);
                                }
                            }
                        }
                    }

                    // make a note of the number of pages that have been crawled and verified.
                    model.IndexedPagesTotal = ResultsDictionary.Count();
                    StoreModelInCache(model);
                    // store the results in the cache
                    StoreResultsInCache(ResultsDictionary, LinksFound, BrokenLinks, Domains);
                }
                catch (Exception ex)
                {
                    // If an Exception occurs then stop the crawl and store the results up till now.
                    model.DataBeingGenerated = false;
                    model.CachedDataAvailable = true;
                    model.ErrorOccured = ex.InnerException.Message;
                    StoreModelInCache(model);
                    break;
                }
            }

            // Now the Crawl has ended, set the view model booleans to let the view know that data is no longer being generated, and their is cached data to view.
            model.DataBeingGenerated = false;
            model.CachedDataAvailable = true;
            StoreModelInCache(model);
        }

        public CrawlerModel ProcessPage(InBoundLinkCheckerViewModel model, List<Examine.SearchResult> Results)
        {
            var CrawlModel = new CrawlerModel();
            if (!model.SiteUri.Contains("http"))
            {
                model.SiteUri = string.Format("{0}{1}", "https://", model.SiteUri);
            }
            // ensure that the we have the current umbraco context ( needed for async methods )
            var context = GetUmbracoContext();

            // Potential to be passed several results.
            foreach (var node in Results)
            {
                CrawlModel.CrawledLinks++;

                WebClient client = new WebClient();
                var doc = new HtmlAgilityPack.HtmlDocument();
                // Almost all the time the urlName is different from the published url
                // search for IPublishedContent to get the published url (this process is significantly slower but much more accurate)
                var TypedContent = new UmbracoHelper(context).TypedContent(int.Parse(node.Fields["__NodeId"]));

                try
                {
                    if (TypedContent != null)
                    {
                        doc.LoadHtml(client.DownloadString(string.Format("{0}{1}", model.SiteUri, TypedContent.Url())));
                        if (!CrawlModel.ResultsDictionary.Keys.Contains(string.Format("{0}{1}", model.SiteUri, TypedContent.Url())))
                        {
                            CrawlModel.ResultsDictionary.Add(string.Format("{0}{1}", model.SiteUri, TypedContent.Url()), new ContentModel(node.Fields["nodeName"], string.Format("{0}{1}", model.SiteUri, TypedContent.Url()), int.Parse(node.Fields["__NodeId"])));
                        }
                    }
                    else
                    {
                        doc.LoadHtml(client.DownloadString(string.Format("{0}/{1}", model.SiteUri, node.Fields["urlName"])));
                        if (!CrawlModel.ResultsDictionary.Keys.Contains(string.Format("{0}/{1}", model.SiteUri, node.Fields["urlName"])))
                        {
                            CrawlModel.ResultsDictionary.Add(string.Format("{0}/{1}", model.SiteUri, node.Fields["urlName"]), new ContentModel(node.Fields["nodeName"], string.Format("{0}/{1}", model.SiteUri, node.Fields["urlName"]), int.Parse(node.Fields["__NodeId"])));
                        }
                    }

                }
                catch (Exception e)
                {
                    // this shouldn't happen but if it does then the node is invalid or the link is broken and should be skipped
                    if (TypedContent != null)
                    {
                        CrawlModel.BrokenLinks.Add(new BrokenPageModel(string.Format("{0}{1}", model.SiteUri, TypedContent.Url()), "Internal Crawl", e.Message));
                    }
                    else
                    {
                        CrawlModel.BrokenLinks.Add(new BrokenPageModel(string.Format("{0}/{1}", model.SiteUri, node.Fields["urlName"]), "Internal Crawl", e.Message));
                    }
                    continue;
                }
                if (TypedContent != null)
                {
                    CrawlModel = GetLinksOnPage(doc.DocumentNode.InnerHtml, string.Format("{0}{1}", model.SiteUri, TypedContent.Url()), CrawlModel);
                }
                else
                {
                    CrawlModel = GetLinksOnPage(doc.DocumentNode.InnerHtml, string.Format("{0}/{1}", model.SiteUri, node.Fields["urlName"]), CrawlModel);
                }
            }
            return CrawlModel;
        }

        private CrawlerModel GetLinksOnPage(string InnerHtml, string Url, CrawlerModel CrawlModel)
        {
            // if possible, only grab data after a body tag
            try
            {
                InnerHtml = InnerHtml.Substring(InnerHtml.IndexOf("<body>"));
            }
            catch (Exception)
            {

            }
            // split the html string into lines by its white space
            var SplitHtml = InnerHtml.Split(' ');
            var HtmlWithLinks = SplitHtml.Where(x => x.Contains("http"));

            foreach (var line in HtmlWithLinks)
            {
                // for each further line, replace characters that surround a link with whitespace.
                var cleanerstring = line.Replace("<", " ").Replace(">", " ").Replace("[", " ").Replace("\n", " ").Replace("]", " ").Replace("'", " ").Replace("\"", " ");
                // split the string again
                var splitCleanString = cleanerstring.Split(' ');

                // use linq to get any lines that containt 'http'
                var links = splitCleanString.Where(x => x.Contains("http"));

                foreach (var item in links)
                {
                    // Make sure the link isn't identical to the page we are on. (Sometimes we find a pages link in meta tags)
                    if (item != Url)
                    {
                        // Double check the line starts with 'http'
                        if (item.StartsWith("http"))
                        {
                            try
                            {
                                if (!CrawlModel.LinksFound.Contains(item))
                                {
                                    // before adding the link to the results or the linksfound list, try to make a uri out of the text.
                                    // if an exception is thrown then the text was an invalid link and skip past adding it to the list or results.   
                                    Uri uri = new Uri(item);
                                    if (!CrawlModel.Domains.Contains(uri.Host))
                                    {
                                        CrawlModel.Domains.Add(uri.Host);
                                    }
                                    CrawlModel.LinksFound.Add(item);
                                }
                            }
                            catch (Exception ex)
                            {
                                // in some cases text that contains http but is not a link can still make it through. If we try to make a uri from that text and it fails. Then we end up here and the text should be skipped.
                                continue;
                            }
                            CrawlModel.ResultsDictionary[Url].LinksOnNode.Add(item);
                        }
                    }
                }
            }
            return CrawlModel;
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

        #endregion

        #region Cache
        private void StoreResultsInCache(Dictionary<string, ContentModel> results, List<string> LinksFound, List<BrokenPageModel> BrokenLinks, List<string> Domains)
        {
            if (cache.Contains("ResultsDictionary"))
            {
                cache.Remove("ResultsDictionary");
            }
            cache.Add("ResultsDictionary", results, DateTime.Now.AddHours(1));

            if (cache.Contains("LinksFound"))
            {
                cache.Remove("LinksFound");
            }
            cache.Add("LinksFound", LinksFound, DateTime.Now.AddHours(1));

            if (cache.Contains("BrokenLinks"))
            {
                cache.Remove("BrokenLinks");
            }
            cache.Add("BrokenLinks", BrokenLinks, DateTime.Now.AddHours(1));

            if (cache.Contains("Domains"))
            {
                cache.Remove("Domains");
            }
            cache.Add("Domains", Domains, DateTime.Now.AddHours(1));
        }

        private void StoreModelInCache(InBoundLinkCheckerViewModel model)
        {
            if (cache.Contains("InBoundLinkCheckerViewModel"))
            {
                cache.Remove("InBoundLinkCheckerViewModel");
            }
            cache.Add("InBoundLinkCheckerViewModel", model, DateTime.Now.AddHours(1));
        }
        #endregion

        #region Validation Crawl
        // The following was meant to mimic the internal crawl but validate the links found on each page instead of just gather them.
        // The code is unstable and returns mixed results when trying to validate links to external pages.
        /*
        public async Task StartValidationCrawl(InBoundLinkCheckerViewModel model)
        {
            var ResultsDictionary = cache["ResultsDictionary"] as Dictionary<string, ContentModel>;

            model.TotalToCrawl = ResultsDictionary.Count();
            model.CrawledLinks = 0;
            model.LinksBeingValidated = true;
            StoreModelInCache(model);

            // run async so as not to present the user with a long loading screen and to return the index view.
            Task.Run(() => ManageValidationCrawl(model));

            Index();
        }


        public async Task ManageValidationCrawl(InBoundLinkCheckerViewModel model)
        {
            var PagesToValidate = new List<ContentModel>();
            var Results = cache["ResultsDictionary"] as Dictionary<string, ContentModel>;
            foreach (var item in Results)
            {
                PagesToValidate.Add(item.Value);
            }

            foreach (var Page in PagesToValidate)
            {
                var LinksOnPage = Page.LinksOnNode.ToList();
                var LinksAvailableToCrawl = true;
                var ThreadCount = 0;
                var ThreadID = 0;
                var ThreadStatus = new Dictionary<int, string>();
                var ThreadList = new Dictionary<int, Task<CrawlerModel>>();
                var ResultsModelList = new List<CrawlerModel>();

                while (LinksAvailableToCrawl)
                {
                    var ResultsDictionary = cache["ResultsDictionary"] as Dictionary<string, ContentModel>;
                    if (ResultsDictionary == null)
                    {
                        ResultsDictionary = new Dictionary<string, ContentModel>();
                    }

                    var ValidatedLinks = new List<string>();
                    foreach (var item in ResultsDictionary)
                    {
                        ValidatedLinks.Add(item.Key);
                    }

                    var BrokenLinks = cache["BrokenLinks"] as List<BrokenPageModel>;
                    if (BrokenLinks == null)
                    {
                        BrokenLinks = new List<BrokenPageModel>();
                    }
                    var LinksFound = cache["LinksFound"] as List<string>;
                    if (LinksFound == null)
                    {
                        LinksFound = new List<string>();
                    }
                    var Domains = cache["Domains"] as List<string>;
                    if (Domains == null)
                    {
                        Domains = new List<string>();
                    }

                    try
                    {
                        while (ThreadCount < 8 && LinksOnPage.Count > 0)
                        {
                            ThreadCount++;
                            ThreadID++;
                            ThreadList.Add(ThreadID, Task.Run(() => ValidatePageLink(LinksOnPage[0], Page.URL, ValidatedLinks, BrokenLinks)));
                            LinksOnPage.RemoveRange(0, 1);
                            ThreadStatus.Add(ThreadID, "Started");
                        }
                        if (LinksOnPage.Count() <= 0) LinksAvailableToCrawl = false;

                        foreach (var Thread in ThreadList)
                        {
                            if (Thread.Value.IsCompleted)
                            {
                                ResultsModelList.Add(Thread.Value.Result);
                                ThreadStatus[Thread.Key] = "Completed";
                            }
                        }

                        var KeysToRemove = new List<int>();
                        foreach (var Thread in ThreadStatus)
                        {
                            if (Thread.Value == "Completed")
                            {
                                ThreadList.Remove(Thread.Key);
                                ThreadCount--;
                                KeysToRemove.Add(Thread.Key);
                            }
                        }
                        foreach (var Key in KeysToRemove)
                        {
                            ThreadStatus.Remove(Key);
                        }

                        if (LinksAvailableToCrawl == false)
                        {
                            foreach (var ResultModel in ResultsModelList)
                            {
                                if (ResultModel != null)
                                {

                                    foreach (var item in ResultModel.BrokenLinks)
                                    {
                                        if (!BrokenLinks.Contains(item))
                                        {
                                            BrokenLinks.Add(item);
                                        }
                                    }
                                    foreach (var item in ResultModel.ResultsDictionary)
                                    {
                                        if (!ResultsDictionary.Keys.Contains(item.Key))
                                        {
                                            ResultsDictionary.Add(item.Key, item.Value);
                                        }
                                    }
                                    foreach (var item in ResultModel.Domains)
                                    {
                                        if (!Domains.Contains(item))
                                        {
                                            Domains.Add(item);
                                        }
                                    }
                                }
                            }
                        }

                        model.IndexedPagesTotal = ResultsDictionary.Count();
                        StoreModelInCache(model);
                        // store the results in the cache
                        StoreResultsInCache(ResultsDictionary, LinksFound, BrokenLinks, Domains);
                    }
                    catch (Exception ex)
                    {
                        model.DataBeingGenerated = false;
                        model.CachedDataAvailable = true;
                        StoreModelInCache(model);
                    }
                }
                model.CrawledLinks++;
            }

            model.LinksBeingValidated = false;
            StoreModelInCache(model);
        }

        private CrawlerModel ValidatePageLink(string URL, string FoundOn, List<string> ValidatedLinks, List<BrokenPageModel> BrokenLinks)
        {
            var CrawlModel = new CrawlerModel();
            WebClient client = new WebClient();
            var doc = new HtmlDocument();
            try
            {

                var AlreadyBroken = BrokenLinks.FirstOrDefault(x => x.URL == URL);

                if (AlreadyBroken == null)
                {
                    if (!CrawlModel.ResultsDictionary.Keys.Contains(URL) && !ValidatedLinks.Contains(URL))
                    {
                        doc.LoadHtml(client.DownloadString(URL));
                        CrawlModel.ResultsDictionary.Add((URL), new ContentModel("External Page", URL));
                    }
                }
                else
                {
                    if (AlreadyBroken.FoundOn != FoundOn)
                    {
                        CrawlModel.BrokenLinks.Add(new BrokenPageModel(URL, FoundOn, AlreadyBroken.Exception));
                    }
                }
            }
            catch (Exception e)
            {
                // this shouldn't happen but if it does then the node is invalid or the link is broken and should be skipped
                CrawlModel.BrokenLinks.Add(new BrokenPageModel(URL, FoundOn, e.Message));
            }
            return CrawlModel;
        }
        */
        #endregion
    }
}