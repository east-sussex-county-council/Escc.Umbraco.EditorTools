using Examine;
using umbraco.BusinessLogic;
using Umbraco.Core;
using UmbracoExamine;

namespace Escc.Umbraco.EditorTools.Events
{
    public class ExamineEvents : ApplicationBase
    {
        public ExamineEvents()
        {
            ExamineManager.Instance.IndexProviderCollection["InternalIndexer"].GatheringNodeData += InternalExamineEvents_GatheringNodeData;
        }

        void InternalExamineEvents_GatheringNodeData(object sender, IndexingNodeDataEventArgs e)
        {
            if (e.IndexType == IndexTypes.Content)
            {
                var contentService = ApplicationContext.Current.Services.ContentService;
                var node = contentService.GetById(e.NodeId);
                e.Fields.Add("isDeleted", node.Trashed.ToString().ToLowerInvariant());
                e.Fields.Add("customIsPublished", node.Published.ToString().ToLowerInvariant());

                if (node.ExpireDate.HasValue)
                {
                    e.Fields.Add("customExpireDate", node.ExpireDate.Value.ToIsoString());
                }
                else
                {
                    e.Fields.Remove("customExpireDate");
                }
            }
        }
    }
}