using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels
{
    public class InBoundLinkCheckerViewModel
    {
        public bool CachedDataAvailable { get; set; }
        public bool DataBeingGenerated { get; set; }
        public bool HasInBoundLinks { get; set; }

        public int CrawledLinks { get; set; }
        public int TotalToCrawl { get; set; }
        public int IndexedPagesTotal { get; set; }

        public string Query { get; set; }
        public string SiteUri { get; set; }
        public Dictionary<int, string> FailedLinks { get; set; }

        public TableModel InBoundLinks { get; set; }

        public InBoundLinkCheckerViewModel()
        {
            HasInBoundLinks = false;
            InBoundLinks = new TableModel("InBoundLinksTable");
        }
    }
}