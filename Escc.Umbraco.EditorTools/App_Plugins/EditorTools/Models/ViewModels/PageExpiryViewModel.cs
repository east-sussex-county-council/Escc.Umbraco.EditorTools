using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels
{
    public class PageExpiryViewModel
    {
        public TableModel NeverExpires { get; set; }
        public TableModel Expiring { get; set; }
        public TableModel RecentlyExpired { get; set; }

        public int TotalExpiring { get; set; }
        public int TotalNeverExpires { get; set; }
        public int TotalExpiresIn14Days { get; set; }
        public int TotalRecentlyExpired { get; set; }

        public DateTime CacheDate { get; set; }
        public bool CachedDataAvailable { get; set; }

        public PageExpiryViewModel()
        {
            CachedDataAvailable = true;
            NeverExpires = new TableModel("NeverExpiresTable");
            Expiring = new TableModel("ExpiringTable");
            RecentlyExpired = new TableModel("RecentlyExpiredTable");
            CacheDate = DateTime.Now;
        }
    }
}