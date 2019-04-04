using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels
{
    public class StatsViewModel
    {
        public MediaViewModel Media { get; set; }
        public bool MediaStatsAvailable { get; set; }

        public ContentViewModel Content { get; set; }
        public bool ContentStatsAvailable { get; set; }

        public InBoundLinkCheckerViewModel Crawler { get; set; }
        public bool CrawlerStatsAvailable { get; set; }
    }
}