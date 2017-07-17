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
        public string URL { get; set; }
        public List<string> LinksOnNode { get; set; }

        public ContentModel(string nodeName, string url)
        {
            NodeName = nodeName;
            URL = url;
            LinksOnNode = new List<string>();
        }
        public ContentModel()
        {

        }
    }
}