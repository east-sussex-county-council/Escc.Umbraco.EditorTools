using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels
{
    public class ContentModel
    {
        public int NodeID { get; set; }
        public string NodeName { get; set; }
        public string NodeType { get; set; }
        public string URL { get; set; }
        public List<string> LinksOnNode { get; set; }

        public ContentModel(int nodeID , string nodeName, string nodeType, string url)
        {
            NodeID = nodeID;
            NodeName = nodeName;
            NodeType = nodeType;
            URL = url;
            LinksOnNode = new List<string>();
        }
    }
}