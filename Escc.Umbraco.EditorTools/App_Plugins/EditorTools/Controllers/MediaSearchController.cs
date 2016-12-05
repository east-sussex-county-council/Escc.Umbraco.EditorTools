using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using Umbraco.Web.WebApi;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class MediaSearchController : UmbracoAuthorizedApiController
    {

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
            mediaList.Clear();
            populateList();
            StoreInCache();
            return mediaList;
        }

        public string GetReportDate()
        {
            return cache["reportDate"] as string;
        }

        private void populateList()
        {
            var rootMedia = UmbracoContext.Application.Services.MediaService.GetRootMedia();
            var userService = ApplicationContext.Services.UserService;

            foreach (var node in rootMedia)
            {
                var descendants = UmbracoContext.Application.Services.MediaService.GetDescendants(node.Id);
                var creator = userService.GetUserById(node.CreatorId);

                mediaList.Add(new Media(node.Name, node.ContentType.Name, node.CreateDate.ToString(), node.Id, creator.Name));

                foreach (var child in descendants)
                {
                    var childCreator = userService.GetUserById(child.CreatorId);
                    mediaList.Add(new Media(child.Name, child.ContentType.Name, child.CreateDate.ToString(), child.Id, childCreator.Name));
                }
            }
        }
    }
}