using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels
{
    public class MediaViewModel
    {
        public int TotalMediaFiles { get; set; }
        public int TotalFolders { get; set; }
        public int TotalImages { get; set; }
        public int TotalFiles { get; set; }
        public TableModel Media { get; set; }
        public TableModel MediaFileTypes { get; set; }
        public DateTime CacheDate { get; set; }
        public bool CachedDataAvailable { get; set; }

        public MediaViewModel()
        {
            CachedDataAvailable = true;
            MediaFileTypes = new TableModel("MediaFileTypesTable");
            Media = new TableModel("MediaTable");
            CacheDate = DateTime.Now;
        }
    }
}