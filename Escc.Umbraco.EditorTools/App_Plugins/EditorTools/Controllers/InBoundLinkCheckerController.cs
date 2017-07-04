using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using UmbracoExamine;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class InBoundLinkCheckerController : UmbracoAuthorizedController
    {
        ObjectCache cache = MemoryCache.Default;
        public ActionResult Index()
        {
            var model = cache["InBoundLinkCheckerViewModel"] as InBoundLinkCheckerViewModel;

            if(model == null)
            {
                model = new InBoundLinkCheckerViewModel();
                model.CachedDataAvailable = false;
                model.DataBeingGenerated = false;
                StoreModelInCache(model);
            }

            return View("~/App_Plugins/EditorTools/Views/InBoundLinkChecker/Index.cshtml", model);
        }


        public ActionResult SearchForInBoundLinks(InBoundLinkCheckerViewModel model)
        {
            model.HasInBoundLinks = false;
            var results = cache["ResultsDictionary"] as Dictionary<int, ContentModel>;

            model.InBoundLinks.Table = new DataTable();
            model.InBoundLinks.Table.Columns.Add("ID", typeof(string));
            model.InBoundLinks.Table.Columns.Add("Name", typeof(string));
            model.InBoundLinks.Table.Columns.Add("Url", typeof(string));
            model.InBoundLinks.Table.Columns.Add("Edit", typeof(HtmlString));

            foreach (var node in results)
            {
                foreach (var link in node.Value.LinksOnNode)
                {
                    if(model.Query == link)
                    {
                        var editURL = new HtmlString(string.Format("<a target=\"_top\" href=\"/umbraco#/content/content/edit/{0}\">edit</a>", node.Key));
                        model.InBoundLinks.Table.Rows.Add(node.Key, node.Value.NodeName, node.Value.URL, editURL);
                    }
                    else if (model.Query.Replace("www" , "new") == link)
                    {
                        var editURL = new HtmlString(string.Format("<a target=\"_top\" href=\"/umbraco#/content/content/edit/{0}\">edit</a>", node.Key));
                        model.InBoundLinks.Table.Rows.Add(node.Key, node.Value.NodeName, node.Value.URL, editURL);
                    }
                }

            }
            if (model.InBoundLinks.Table.Rows.Count > 0)
            {
                model.HasInBoundLinks = true;
            }
            return View("~/App_Plugins/EditorTools/Views/InBoundLinkChecker/Index.cshtml", model);
        }

        public async Task<ActionResult> StartCrawl(InBoundLinkCheckerViewModel model)
        {
            model.DataBeingGenerated = true;
            StoreModelInCache(model);

            // run async so as not to present the user with a long loading screen and to return the index view.
            Task.Run(() => GatherPageLinks(model));

            return View("~/App_Plugins/EditorTools/Views/InBoundLinkChecker/Index.cshtml", model);
        }

        public void GatherPageLinks(InBoundLinkCheckerViewModel model)
        {

            // Create a dictionary to store the results
            var ResultsDictionary = new Dictionary<int, ContentModel>();

            // Create a dictionary to store failed links
            var FailedLinks = new Dictionary<int, string>();

            // instantiate the examine search and its criteria type
            var Searcher = Examine.ExamineManager.Instance.SearchProviderCollection["ExternalSearcher"];
            var Criteria = Searcher.CreateSearchCriteria(IndexTypes.Content);

            // Create a query to get all nodes of the index type content
            var ExamineQuery = Criteria.RawQuery(string.Format("__IndexType:content"));
            var Results = Searcher.Search(ExamineQuery);

            model.TotalToCrawl = Results.Count();

            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();

            //var crawlLength = 10;
            var crawlNo = 0;
            foreach (var node in Results)
            {
                crawlNo ++;
                model.FailedLinks = FailedLinks;
                model.CrawledLinks = crawlNo;
                StoreModelInCache(model);

                WebClient client = new WebClient();
                var doc = new HtmlAgilityPack.HtmlDocument();
                try
                {
                    doc.LoadHtml(client.DownloadString(string.Format("{0}/{1}", model.SiteUri, node.Fields["urlName"])));
                    if (!ResultsDictionary.Keys.Contains(int.Parse(node.Fields["__NodeId"])))
                    {
                        ResultsDictionary.Add(int.Parse(node.Fields["__NodeId"]), new ContentModel(int.Parse(node.Fields["__NodeId"]), node.Fields["nodeName"], node.Fields["nodeTypeAlias"], node.Fields["urlName"]));
                    }
                }
                catch (Exception)
                {
                    // If we end up here then the urlName is different from the published url
                    // search for IPublishedContent to get the published url (this process is significantly slower)
                    try
                    {
                        var content = UmbracoContext.ContentCache.GetById(int.Parse(node.Fields["__NodeId"]));
                        doc.LoadHtml(client.DownloadString(string.Format("{0}{1}", model.SiteUri, content.Url)));
                        if (!ResultsDictionary.Keys.Contains(int.Parse(node.Fields["__NodeId"])))
                        {
                            ResultsDictionary.Add(int.Parse(node.Fields["__NodeId"]), new ContentModel(int.Parse(node.Fields["__NodeId"]), node.Fields["nodeName"], node.Fields["nodeTypeAlias"], content.Url));
                        }
                    }
                    catch (Exception)
                    {
                        // this shouldn't happen but if it does then the node is invalid and should be skipped
                        FailedLinks.Add(int.Parse(node.Fields["__NodeId"]), string.Format("{0}/{1}", model.SiteUri, node.Fields["urlName"]));
                        continue;
                    }
                }


                // get all of the pages html as a string
                var InnerHtml = doc.DocumentNode.InnerHtml;
                // split the html string into lines by its white space
                var SplitHtml = InnerHtml.Split(' ');

                // for each line
                foreach (var Htmlline in SplitHtml)
                {
                    // if the line contains the string http
                    if (Htmlline.Contains("http"))
                    {
                        // split the line by its quotations
                        var SplitLine = Htmlline.Split('"');
                        // for each further split line
                        foreach (var line in SplitLine)
                        {
                            // if the line again containts http, then it is a link
                            if (line.Contains("http"))
                            {
                                // clean up the string and add it to the nodes list of links
                                var cleanLink = line.Replace("<url>", "").Replace("</url>", "").Replace("<link>","").Replace("</link>","").Replace("\n", "");
                                ResultsDictionary[int.Parse(node.Fields["__NodeId"])].LinksOnNode.Add(cleanLink);
                            }
                        }
                    }
                }
                //if (crawlNo >= crawlLength) break;
            }
            // store the results in the cache
            StoreResultsInCache(ResultsDictionary);  
        }


        private void StoreResultsInCache(Dictionary<int, ContentModel> results)
        {
            cache.Add("ResultsDictionary", results, System.Web.Caching.Cache.NoAbsoluteExpiration, null);

            var model = cache["InBoundLinkCheckerViewModel"] as InBoundLinkCheckerViewModel;
            model.DataBeingGenerated = false;
            model.CachedDataAvailable = true;
            model.IndexedPagesTotal = results.Count();
            StoreModelInCache(model);
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
    }
}