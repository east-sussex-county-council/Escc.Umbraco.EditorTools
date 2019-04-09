using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels
{
    public class MediaViewModel
    {
        public string Query { get; set; }
        public int TotalMediaFiles { get; set; }
        public int TotalFolders { get; set; }
        public int TotalImages { get; set; }
        public int TotalFiles { get; set; }
        public TableModel Media { get; set; }
        public TableModel MediaFileTypes { get; set; }
        public DateTime CacheDate { get; set; }
        public bool StatisticsDataAvailable { get; set; }
        public bool HasMediaResults { get; set; }
        public bool ShowStatistics { get; set; }

        public MediaViewModel()
        {
            StatisticsDataAvailable = false;
            MediaFileTypes = new TableModel("MediaFileTypesTable");
            Media = new TableModel("MediaTable");
            CacheDate = DateTime.Now;
            HasMediaResults = false;
        }
    }
}