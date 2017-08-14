using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels
{
    public class ExpiryBulkUpdateViewModel
    {
        public List<MultiMoveContentModel> Content { get; set; }
        public Dictionary<int, int> ContentLevel { get; set; }
        public string SuccessMessage { get; set; }
        public string WarningMessage { get; set; }
        public string ErrorMessage { get; set; }

        public ExpiryBulkUpdateViewModel()
        {
            Content = new List<MultiMoveContentModel>();
            ContentLevel = new Dictionary<int, int>();
        }
    }
}