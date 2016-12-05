using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models;
using System.Collections.Generic;
using Umbraco.Web.WebApi;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class MediaSearchController : UmbracoAuthorizedApiController
    {
        // create a list to store users
        List<Media> mediaList = new List<Media>();

        public IEnumerable<Media> GetAllMedia()
        {
            var rootMedia = UmbracoContext.Application.Services.MediaService.GetRootMedia();

            foreach (var node in rootMedia)
            {
                var descendants = UmbracoContext.Application.Services.MediaService.GetDescendants(node.Id);
                mediaList.Add(new Media(node.Name, node.ContentType.Name, node.CreateDate.ToString(), node.Id));

                foreach (var child in descendants)
                {
                    mediaList.Add(new Media(child.Name, child.ContentType.Name, child.CreateDate.ToString(), child.Id));
                }
            }
            return mediaList;
        }
    }
}