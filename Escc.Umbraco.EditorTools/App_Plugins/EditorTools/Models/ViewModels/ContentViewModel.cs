using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Web;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels
{
    public class ContentViewModel
    {
        public string Query { get; set; }

        public TableModel Content { get; set; }

        public bool HasContentResults { get; set; }

        public TableModel PublishedContent { get; set; }
        public TableModel UnpublishedContent { get; set; }
        public TableModel DocumentTypes { get; set; }
        public Dictionary<string, TableModel> ModalTables { get; set; }
        public DateTime CacheDate { get; set; }
        public int PublishedPages { get; set; }
        public int UnpublishedPages { get; set; }
        public int TotalPages { get; set; }
        
        public bool CachedDataAvailable { get; set; }
        public string Tab { get; internal set; }

        public ContentViewModel()
        {
            CachedDataAvailable = true;
            ModalTables = new Dictionary<string, TableModel>();
            Content = new TableModel("ContentTable");
            HasContentResults = false;
            DocumentTypes = new TableModel("DocumentsOfTypeTable");
            PublishedContent = new TableModel("PublishedContentTable");
            UnpublishedContent = new TableModel("UnpublishedContentTable");
            CacheDate = DateTime.Now;
        }
    }
}