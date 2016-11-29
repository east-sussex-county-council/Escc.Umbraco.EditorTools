using System.Collections.Generic;
using Umbraco.Web.WebApi;
using Escc.Umbraco.EditorTools.Models;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class PageExpiryReportController : UmbracoAuthorizedApiController
    {
        // create a list to store the non perishable pages
        List<NonPerishableContent> nonPerishableList = new List<NonPerishableContent>();

        public IEnumerable<NonPerishableContent> GetAllNonPerishable()
        {
            // get all content at the root of the content tree
            var rootCollection = UmbracoContext.ContentCache.GetAtRoot();
            // for each node at the root
            foreach (var node in rootCollection)
            {
                // get the nodes descendants
                var descendants = UmbracoContext.Application.Services.ContentService.GetDescendants(node.Id);

                // for each descendant
                foreach (var child in descendants)
                {
                    // if the expiry date is null e.g. set not to expire
                    if (child.ExpireDate == null)
                    {
                        // get the node from the contentcache
                        var publishedNode = UmbracoContext.ContentCache.GetById(child.Id);
                        // if the node is not published
                        if (publishedNode == null)
                        {
                            // add the node to the list, but with the url set to 'unpublished'
                            nonPerishableList.Add(new NonPerishableContent(child.Id, child.Name, "unpublished"));
                        }
                        // if the node is published
                        else
                        {
                            // add that nodes id, name and url to the content list
                            nonPerishableList.Add(new NonPerishableContent(child.Id, child.Name, publishedNode.Url));
                        }
                    }
                }
            }

            return nonPerishableList;
        }
    }
}