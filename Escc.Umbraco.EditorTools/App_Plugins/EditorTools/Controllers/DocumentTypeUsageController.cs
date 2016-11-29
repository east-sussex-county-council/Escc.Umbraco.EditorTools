using Escc.Umbraco.EditorTools.Models;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Web;
using Umbraco.Web.WebApi;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class DocumentTypeUsageController : UmbracoAuthorizedApiController
    {
        // create lists to store content and document types
        List<Content> contentList = new List<Content>();
        List<DocumentType> documentTypeList = new List<DocumentType>();

       
        public IEnumerable<DocumentType> GetAllDocumentTypes()
        {
            PopulateDocumentTypeList();
            return documentTypeList;
        }

        public IEnumerable<Content> GetAllContent(int typeId, string typeName)
        {
            PopulateContentList(typeId, typeName);
            return contentList;
        }



        private void PopulateDocumentTypeList()
        {
            // create a collection of all content types in the application context
            // this will include content types that have yet to be published
            var contentCollection = ApplicationContext.Services.ContentTypeService.GetAllContentTypes();
            foreach (var item in contentCollection)
            {
                // count how many times a content type has been used
                var count = ApplicationContext.Services.ContentService.Count(item.Alias);
                
                // if a content type has been used at least once
                if (count > 0)
                {
                    // add its id, name and count to the list
                    documentTypeList.Add(new DocumentType(item.Id, item.Name, count));
                }
            }
        }

        private void PopulateContentList(int typeId, string typeName)
        {

            // Get the Nodes of the specified content type as a list
            var nodes = ApplicationContext.Services.ContentService.GetContentOfContentType(typeId).ToList();        

            // for each node in the list
            foreach (var node in nodes)
            {
                // get nodes published information to get the url
                var publishedNode = UmbracoContext.ContentCache.GetById(node.Id);
                
                // if the node is not published
                if (publishedNode == null)
                {
                    // add the node to the list, but with the url set to 'unpublished'
                    contentList.Add(new Content(node.Id, node.Name, typeName, "unpublished"));
                }
                // if the node is published
                else
                {
                    // add that nodes id, name, document type and url to the content list
                    contentList.Add(new Content(node.Id, node.Name, typeName, publishedNode.Url));
                }
            }
        }
    }
}
