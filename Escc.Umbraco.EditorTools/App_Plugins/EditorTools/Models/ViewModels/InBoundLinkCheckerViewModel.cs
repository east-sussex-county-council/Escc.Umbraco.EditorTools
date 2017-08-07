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

        public string ErrorOccured { get; set; }

        public TableModel Domains { get; set; }
        public TableModel BrokenLinks { get; set; }
        public TableModel IndexedLinks { get; set; }
        public TableModel InBoundLinks { get; set; }
        public TableModel LinksFoundTable { get; set; }

        public int TotalUniqueLinks { get; set; }
        public int TotalDomainsFound { get; set; }
        public int TotalBrokenLinks { get; set; }
        public int TotalVerified { get; set; }



        public InBoundLinkCheckerViewModel()
        {
            Domains = new TableModel("DomainsTable");
            HasInBoundLinks = false;
            IndexedLinks = new TableModel("IndexedLinksTable");
            BrokenLinks = new TableModel("BrokenLinksTable");
            InBoundLinks = new TableModel("InBoundLinksTable");
            LinksFoundTable = new TableModel("LinksFoundTable");
        }
    }
}