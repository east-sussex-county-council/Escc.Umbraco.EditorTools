using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using Umbraco.Web.WebApi;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class MediaSearchController : UmbracoAuthorizedApiController
    {
        // instantiate cache to reduce data queries
        ObjectCache cache = MemoryCache.Default;
        // create a list to store media
        List<Media> mediaList = new List<Media>();

        public IEnumerable<Media> GetAllMedia()
        {
            // check the cache to see if the list already exists
            mediaList = cache["MediaList"] as List<Media>;
            // if it doesnt exist then generate the list
            if (mediaList == null)
            {
                mediaList = new List<Media>();
                populateList();
                StoreInCache();
            }
            return mediaList;
        }

        private void StoreInCache()
        {
            // add the list and date generated to the cache 
            var dateString = DateTime.Now.ToString("dd MMMM yyyy");
            dateString += DateTime.Now.ToString(" h:mm:sstt").ToLower();
            cache.Add("MediaList", mediaList, System.Web.Caching.Cache.NoAbsoluteExpiration, null);
            cache.Add("reportDate", dateString, System.Web.Caching.Cache.NoAbsoluteExpiration, null);
        }

        public IEnumerable<Media> GetMediaRefresh()
        {
            // clear the list
            mediaList.Clear();
            // repopulate
            populateList();
            // update the cache
            StoreInCache();
            return mediaList;
        }

        public string GetReportDate()
        {
            return cache["reportDate"] as string;
        }

        private void populateList()
        {
            // get all media at the root
            var rootMedia = UmbracoContext.Application.Services.MediaService.GetRootMedia();
            // instantiate user service to find creator
            var userService = ApplicationContext.Services.UserService;

            // for each node in the root
            foreach (var node in rootMedia)
            {
                // get nodes descendants
                var descendants = UmbracoContext.Application.Services.MediaService.GetDescendants(node.Id);
                // get the nodes creator
                var creator = userService.GetUserById(node.CreatorId);
                // add node to the list
                mediaList.Add(new Media(node.Name, node.ContentType.Name, node.CreateDate.ToString(), node.Id, creator.Name));

                // for each child in descendants
                foreach (var child in descendants)
                {
                    // get childs creator
                    var childCreator = userService.GetUserById(child.CreatorId);
                    // add child to the list
                    mediaList.Add(new Media(child.Name, child.ContentType.Name, child.CreateDate.ToString(), child.Id, childCreator.Name));
                }
            }
        }
    }
}