using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
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
        ObjectCache cache = MemoryCache.Default;

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
            else
            {
                // if a crawl has been done and their is cached data avialable, then prepare the viewmodel.
                if (model.CachedDataAvailable)
                {
                    model = PrepareViewModel(model);
                }
            }


            return View("~/App_Plugins/EditorTools/Views/InBoundLinkChecker/Index.cshtml", model);
        }

        public ActionResult SearchForInBoundLinks(InBoundLinkCheckerViewModel PostModel)
        {
            var model = cache["InBoundLinkCheckerViewModel"] as InBoundLinkCheckerViewModel;
            model.Query = PostModel.Query;
            model.HasInBoundLinks = false;
            var results = cache["ResultsDictionary"] as Dictionary<string, ContentModel>;

            model.InBoundLinks.Table = new DataTable();
            model.InBoundLinks.Table.Columns.Add("ID", typeof(string));
            model.InBoundLinks.Table.Columns.Add("Name", typeof(string));
            model.InBoundLinks.Table.Columns.Add("Url", typeof(string));
            model.InBoundLinks.Table.Columns.Add("Edit", typeof(HtmlString));

            var PagesWithLink = new List<ContentModel>();
            foreach (var node in results)
            {
                foreach (var link in node.Value.LinksOnNode)
                {
                    if (model.Query == link)
                    {
                        if (!PagesWithLink.Contains(node.Value))
                        {
                            PagesWithLink.Add(node.Value);
                        }
                    }
                    else if (model.Query.Replace("www", "new") == link)
                    {
                        if (!PagesWithLink.Contains(node.Value))
                        {
                            PagesWithLink.Add(node.Value);
                        }
                    }
                }

            }
            if (PagesWithLink.Count > 0)
            {
                model.HasInBoundLinks = true;
                foreach (var page in PagesWithLink)
                {
                    var editURL = new HtmlString(string.Format("<a target=\"_top\" href=\"/umbraco#/content/content/edit/{0}\">edit</a>", page.NodeID));
                    model.InBoundLinks.Table.Rows.Add(page.NodeID, page.NodeName, page.URL, editURL);
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

            model.TotalToCrawl = Results.Count();
            model.CrawledLinks = 0;
            model.DataBeingGenerated = true;
            StoreModelInCache(model);

            Task.Run(() => ManageCrawl(model, Results.ToList()));

            this.HttpContext.Response.AddHeader("refresh", "2; url=" + Url.Action("Index"));
            return View("~/App_Plugins/EditorTools/Views/InBoundLinkChecker/Index.cshtml", model);
        }

        public async Task<ActionResult> StartExternalCrawl(InBoundLinkCheckerViewModel Postmodel)
        {
            var model = cache["InBoundLinkCheckerViewModel"] as InBoundLinkCheckerViewModel;
            model.ExternalDataBeingGenerated = true;
            StoreModelInCache(model);

            // run async so as not to present the user with a long loading screen and to return the index view.
            Task.Run(() => GatherExternalPageLinks(model));
            return View("~/App_Plugins/EditorTools/Views/InBoundLinkChecker/Index.cshtml", model);
        }

        #endregion

        #region Helpers

        public async Task ManageCrawl(InBoundLinkCheckerViewModel model, List<Examine.SearchResult> Results)
        {
            var count = 1;
            while (Results.Count > 0)
            {
                try
                {
                    var Results1 = Results.Take(1).ToList();
                    var Results2 = new List<Examine.SearchResult>();
                    var Results3 = new List<Examine.SearchResult>();
                    var Results4 = new List<Examine.SearchResult>();
                    var Results5 = new List<Examine.SearchResult>();
                    var Results6 = new List<Examine.SearchResult>();
                    var Results7 = new List<Examine.SearchResult>();
                    var Results8 = new List<Examine.SearchResult>();
                    var Results9 = new List<Examine.SearchResult>();
                    var Results10 = new List<Examine.SearchResult>();

                    if (Results.Count > 1) { Results2 = Results.Skip(1).Take(1).ToList(); }
                    if (Results.Count > 2) { Results3 = Results.Skip(2).Take(1).ToList(); }
                    if (Results.Count > 3) { Results4 = Results.Skip(3).Take(1).ToList(); }
                    if (Results.Count > 4) { Results5 = Results.Skip(4).Take(1).ToList(); }
                    if (Results.Count > 5) { Results6 = Results.Skip(5).Take(1).ToList(); }
                    if (Results.Count > 6) { Results7 = Results.Skip(6).Take(1).ToList(); }
                    if (Results.Count > 7) { Results8 = Results.Skip(7).Take(1).ToList(); }
                    if (Results.Count > 8) { Results9 = Results.Skip(8).Take(1).ToList(); }
                    if (Results.Count > 9) { Results10 = Results.Skip(9).Take(1).ToList(); }

                    if(Results.Count < 10)
                    {
                        Results.RemoveRange(0, Results.Count);
                    } 
                    else
                    {
                        Results.RemoveRange(0, 10);
                    }
                   

                    // run async so as not to present the user with a long loading screen and to return the index view.
                    var Thread1 = Task.Run(() => GatherPageLinks(model, Results1));
                    var Thread2 = Results.Count > 1 ? Task.Run(() => GatherPageLinks(model, Results2)) : null;
                    var Thread3 = Results.Count > 2 ? Task.Run(() => GatherPageLinks(model, Results3)) : null;
                    var Thread4 = Results.Count > 3 ? Task.Run(() => GatherPageLinks(model, Results4)) : null;
                    var Thread5 = Results.Count > 4 ? Task.Run(() => GatherPageLinks(model, Results5)) : null;
                    var Thread6 = Results.Count > 5 ? Task.Run(() => GatherPageLinks(model, Results6)) : null;
                    var Thread7 = Results.Count > 6 ? Task.Run(() => GatherPageLinks(model, Results7)) : null;
                    var Thread8 = Results.Count > 7 ? Task.Run(() => GatherPageLinks(model, Results8)) : null;
                    var Thread9 = Results.Count > 8 ? Task.Run(() => GatherPageLinks(model, Results9)) : null;
                    var Thread10 = Results.Count > 9 ? Task.Run(() => GatherPageLinks(model, Results10)) : null;

                    CrawlerModel Result1 = await Thread1;
                    model.CrawledLinks += Result1.CrawledLinks;
                    StoreModelInCache(model);

                    CrawlerModel Result2 = Thread2 == null ? null : await Thread2;
                    if (Result2 != null)
                    {
                        model.CrawledLinks += Result2.CrawledLinks;
                        StoreModelInCache(model);
                    }

                    CrawlerModel Result3 = Thread3 == null ? null : await Thread3;
                    if (Result3 != null)
                    {
                        model.CrawledLinks += Result3.CrawledLinks;
                        StoreModelInCache(model);
                    }

                    CrawlerModel Result4 = Thread4 == null ? null : await Thread4;
                    if (Result4 != null)
                    {
                        model.CrawledLinks += Result4.CrawledLinks;
                        StoreModelInCache(model);
                    }

                    CrawlerModel Result5 = Thread5 == null ? null : await Thread5;
                    if (Result5 != null)
                    {
                        model.CrawledLinks += Result5.CrawledLinks;
                        StoreModelInCache(model);
                    }

                    CrawlerModel Result6 = Thread6 == null ? null : await Thread6;
                    if (Result6 != null)
                    {
                        model.CrawledLinks += Result6.CrawledLinks;
                        StoreModelInCache(model);
                    }

                    CrawlerModel Result7 = Thread7 == null ? null : await Thread7;
                    if (Result7 != null)
                    {
                        model.CrawledLinks += Result7.CrawledLinks;
                        StoreModelInCache(model);
                    }

                    CrawlerModel Result8 = Thread8 == null ? null : await Thread8;
                    if (Result8 != null)
                    {
                        model.CrawledLinks += Result8.CrawledLinks;
                        StoreModelInCache(model);
                    }

                    CrawlerModel Result9 = Thread9 == null ? null : await Thread9;
                    if (Result9 != null)
                    {
                        model.CrawledLinks += Result9.CrawledLinks;
                        StoreModelInCache(model);
                    }

                    CrawlerModel Result10 = Thread10 == null ? null : await Thread10;
                    if (Result10 != null)
                    {
                        model.CrawledLinks += Result10.CrawledLinks;
                        StoreModelInCache(model);
                    }

                    var ResultsModelList = new List<CrawlerModel>(new CrawlerModel[] { Result1, Result2, Result3, Result4, Result5, Result6, Result7, Result8, Result9, Result10 });

                    var ResultsDictionary = cache["ResultsDictionary"] as Dictionary<string, ContentModel>;
                    if (ResultsDictionary == null)
                    {
                        ResultsDictionary = new Dictionary<string, ContentModel>();
                    }
                    var LinksFound = cache["LinksFound"] as List<string>;
                    if (LinksFound == null)
                    {
                        LinksFound = new List<string>();
                    }
                    var BrokenLinks = cache["BrokenLinks"] as List<BrokenPageModel>;
                    if (BrokenLinks == null)
                    {
                        BrokenLinks = new List<BrokenPageModel>();
                    }
                    var Domains = cache["Domains"] as List<string>;
                    if (Domains == null)
                    {
                        Domains = new List<string>();
                    }

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
                            foreach (var item in ResultModel.LinksFound)
                            {
                                if (!LinksFound.Contains(item))
                                {
                                    LinksFound.Add(item);
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
                    model.IndexedPagesTotal = ResultsDictionary.Count();
                    StoreModelInCache(model);
                    // store the results in the cache
                    StoreResultsInCache(ResultsDictionary, LinksFound, BrokenLinks, Domains);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            model.DataBeingGenerated = false;
            model.CachedDataAvailable = true;
            StoreModelInCache(model);
        }

        public CrawlerModel GatherPageLinks(InBoundLinkCheckerViewModel model, List<Examine.SearchResult> Results)
        {
            var CrawlModel = new CrawlerModel();
            if (!model.SiteUri.Contains("http"))
            {
                model.SiteUri = string.Format("{0}{1}", "https://", model.SiteUri);
            }

            // ensure that the we have the current umbraco context ( needed for async methods )
            var context = GetUmbracoContext();

            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();

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
                    doc.LoadHtml(client.DownloadString(string.Format("{0}{1}", model.SiteUri, TypedContent.Url())));
                    if (!CrawlModel.ResultsDictionary.Keys.Contains(string.Format("{0}{1}", model.SiteUri, TypedContent.Url())))
                    {
                        CrawlModel.ResultsDictionary.Add(string.Format("{0}{1}", model.SiteUri, TypedContent.Url()), new ContentModel(node.Fields["nodeName"], string.Format("{0}{1}", model.SiteUri, TypedContent.Url())));
                    }
                }
                catch (Exception e)
                {
                    // this shouldn't happen but if it does then the node is invalid or the link is broken and should be skipped
                    CrawlModel.BrokenLinks.Add(new BrokenPageModel(string.Format("{0}{1}", model.SiteUri, TypedContent.Url()), "Internal Crawl", e.Message));
                    continue;
                }
                CrawlModel = GetLinksOnPage(doc.DocumentNode.InnerHtml, string.Format("{0}{1}", model.SiteUri, TypedContent.Url()), CrawlModel);
            }
            return CrawlModel;
        }

        public InBoundLinkCheckerViewModel PrepareViewModel(InBoundLinkCheckerViewModel model)
        {
            var ResultsDictionary = cache["ResultsDictionary"] as Dictionary<string, ContentModel>;
            var LinksFound = cache["LinksFound"] as List<string>;
            var BrokenLinks = cache["BrokenLinks"] as List<BrokenPageModel>;
            var Domains = cache["Domains"] as List<string>;

            model.IndexedLinks.Table = new DataTable();
            model.IndexedLinks.Table.Columns.Add("Name", typeof(string));
            model.IndexedLinks.Table.Columns.Add("Published Url", typeof(string));

            model.BrokenLinks.Table = new DataTable();
            model.BrokenLinks.Table.Columns.Add("URL", typeof(string));
            model.BrokenLinks.Table.Columns.Add("Found On", typeof(string));
            model.BrokenLinks.Table.Columns.Add("Exception", typeof(string));

            model.LinksFoundTable.Table = new DataTable();
            model.LinksFoundTable.Table.Columns.Add("Link", typeof(string));

            model.Domains.Table = new DataTable();
            model.Domains.Table.Columns.Add("Domain", typeof(string));


            foreach (var item in ResultsDictionary)
            {
                model.IndexedLinks.Table.Rows.Add(item.Value.NodeName, item.Value.URL);
            }
            foreach (var item in LinksFound)
            {
                model.LinksFoundTable.Table.Rows.Add(item);
            }
            foreach (var item in BrokenLinks)
            {
                model.BrokenLinks.Table.Rows.Add(item.URL, item.FoundOn, item.Exception);
            }
            foreach (var item in Domains)
            {
                model.Domains.Table.Rows.Add(item);
            }
            return model;
        }


        public void GatherExternalPageLinks(InBoundLinkCheckerViewModel model)
        {

            var ResultsDictionary = cache["ResultsDictionary"] as Dictionary<string, ContentModel>;
            var LinksFound = cache["LinksFound"] as List<string>;
            var BrokenLinks = cache["BrokenLinks"] as List<BrokenPageModel>;

            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            WebClient client = new WebClient();
            var doc = new HtmlAgilityPack.HtmlDocument();

            var ResultsCopy = ResultsDictionary.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);


            try
            {
                model.TotalExternalToCrawl = 0;
                foreach (var item in ResultsDictionary)
                {
                    model.TotalExternalToCrawl += item.Value.LinksOnNode.Count();
                }

                model.CrawledExternalLinks = 0;
                foreach (var Result in ResultsCopy)
                {
                    foreach (var link in Result.Value.LinksOnNode)
                    {
                        model.CrawledExternalLinks++;
                        StoreModelInCache(model);
                        // if the link isnt already in the results dictionary
                        if (!ResultsDictionary.Keys.Contains(link))
                        {
                            // try to load the page and gather the links on that page.
                            try
                            {
                                doc.LoadHtml(client.DownloadString(link));
                                ResultsDictionary.Add(link, new ContentModel("ExternalPage", link));
                            }
                            catch (Exception ex)
                            {
                                // this shouldn't happen but if it does then the node is invalid or the link is broken and should be skipped
                                BrokenLinks.Add(new BrokenPageModel(link, Result.Value.URL, ex.Message));
                                continue;
                            }
                            StoreModelInCache(model);
                            // StoreResultsInCache(ResultsDictionary, LinksFound, BrokenLinks);
                            // GetLinksOnPage(doc.DocumentNode.InnerHtml, link);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }


            model.ExternalDataBeingGenerated = false;
            model.CachedExternalDataAvailable = true;
            model.IndexedPagesTotal = ResultsDictionary.Count();
            StoreModelInCache(model);
            // store the results in the cache
            //  StoreResultsInCache(ResultsDictionary, LinksFound, BrokenLinks);
        }



        private CrawlerModel GetLinksOnPage(string InnerHtml, string Url, CrawlerModel CrawlModel)
        {
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
            // Ensures the UmbracoContext is available to Async methods.
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
                cache["ResultsDictionary"] = results;
            }
            else
            {
                cache.Add("ResultsDictionary", results, System.Web.Caching.Cache.NoAbsoluteExpiration, null);
            }

            if (cache.Contains("LinksFound"))
            {
                cache["LinksFound"] = LinksFound;
            }
            else
            {
                cache.Add("LinksFound", LinksFound, System.Web.Caching.Cache.NoAbsoluteExpiration, null);
            }

            if (cache.Contains("BrokenLinks"))
            {
                cache["BrokenLinks"] = BrokenLinks;
            }
            else
            {
                cache.Add("BrokenLinks", BrokenLinks, System.Web.Caching.Cache.NoAbsoluteExpiration, null);
            }
            if (cache.Contains("Domains"))
            {
                cache["Domains"] = Domains;
            }
            else
            {
                cache.Add("Domains", Domains, System.Web.Caching.Cache.NoAbsoluteExpiration, null);
            }
        }

        private void StoreModelInCache(InBoundLinkCheckerViewModel model)
        {
            if (cache.Contains("InBoundLinkCheckerViewModel"))
            {
                cache["InBoundLinkCheckerViewModel"] = model;
            }
            else
            {
                cache.Add("InBoundLinkCheckerViewModel", model, System.Web.Caching.Cache.NoAbsoluteExpiration, null);
            }
        }
        #endregion
    }
}