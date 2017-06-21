using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels
{
    public class ExamineSearchViewModel
    {
        public string Query { get; set; }
        public string SearchType { get; set; }

        public TableModel MediaTable { get; set; }
        public TableModel ContentTable { get; set; }

        public bool HasMediaResults { get; set; }
        public bool HasContentResults { get; set; }

        public ExamineSearchViewModel()
        {
            MediaTable = new TableModel("MediaTable");
            ContentTable = new TableModel("ContentTable");
            HasMediaResults = false;
            HasContentResults = false;
        }
    }
}