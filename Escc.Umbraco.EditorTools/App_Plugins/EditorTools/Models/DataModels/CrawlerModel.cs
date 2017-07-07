using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels
{
    public class CrawlerModel
    {
        public Dictionary<string, ContentModel> ResultsDictionary {get;set;}
        public List<string> LinksFound { get; set; }
        public List<BrokenPageModel> BrokenLinks { get; set; }
        public List<string> Domains { get; set; }
        public int CrawledLinks { get; set; }


        public CrawlerModel()
        {
            ResultsDictionary = new Dictionary<string, ContentModel>();
            LinksFound = new List<string>();
            BrokenLinks = new List<BrokenPageModel>();
            Domains = new List<string>();
        }
    }
}