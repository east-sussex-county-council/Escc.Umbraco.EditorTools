using Escc.Umbraco.EditorTools.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using Umbraco.Web;
using Umbraco.Web.WebApi;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class PageExpiryReportController : UmbracoAuthorizedApiController
    {
        // instantiate the application cache to reduce data queries
        ObjectCache cache = MemoryCache.Default;
        // create a list to store the non perishable pages
        List<NonPerishableContent> nonPerishableList = new List<NonPerishableContent>();

        public IEnumerable<NonPerishableContent> GetAllNonPerishable()
        {
            // check the cache to see if the list already exists
            nonPerishableList = cache["pageExpiry"] as List<NonPerishableContent>;

            // if it doesnt exist then generate the list
            if (nonPerishableList == null)
            {
                nonPerishableList = new List<NonPerishableContent>();
                populateList();

                // add the list and date generated to the cache 
                var dateString = DateTime.Now.ToString("dd MMMM yyyy");
                dateString += DateTime.Now.ToString(" h:mm:sstt").ToLower();
                cache.Add("pageExpiry", nonPerishableList, System.Web.Caching.Cache.NoAbsoluteExpiration, null);
                cache.Add("reportDate", dateString, System.Web.Caching.Cache.NoAbsoluteExpiration, null);
            }
            return nonPerishableList;
        }

        public IEnumerable<NonPerishableContent> GetRefreshReport()
        {
            nonPerishableList.Clear();
            populateList();
            return nonPerishableList;
        }

        public string GetReportDate()
        {
            return cache["reportDate"] as string;
        }


        private void populateList()
        {
            // get all content at the root of the content tree
            var rootCollection = UmbracoContext.ContentCache.GetAtRoot();

            // for each node at the root
            foreach (var node in rootCollection)
            {
                // get the root node from the application context to get the expiry date
                var applicationNode = UmbracoContext.Application.Services.ContentService.GetById(node.Id);

                // if the exiry date is null. add the node to the list
                if (applicationNode.ExpireDate == null && applicationNode.HasPublishedVersion)
                {
                    // get the nodes descendants
                    var descendants = UmbracoContext.Application.Services.ContentService.GetDescendants(node.Id);
                    nonPerishableList.Add(new NonPerishableContent(node.Id, node.Name, node.Url));
                    // for each descendant
                    foreach (var child in descendants)
                    {
                        var ContentCacheChild = UmbracoContext.ContentCache.GetById(child.Id);
                        // if the expiry date is null and the child is in the content cache
                        if (child.ExpireDate == null && ContentCacheChild != null)
                        {
                            // get the node from the contentcache
                            // add that nodes id, name and url to the content list
                            nonPerishableList.Add(new NonPerishableContent(child.Id, child.Name, ContentCacheChild.Url));
                        }
                    }
                }
            }
        }
    }
} 