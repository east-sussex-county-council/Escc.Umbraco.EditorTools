using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels
{
    public class MultiMoveViewModel
    {
        public List<MultiMoveContentModel> Content { get; set; }
        public Dictionary<int, int> ContentLevel { get; set; }
        public string Message { get; set; }

        public MultiMoveViewModel()
        {
            Content = new List<MultiMoveContentModel>();
            ContentLevel = new Dictionary<int, int>();
        }
    }
}