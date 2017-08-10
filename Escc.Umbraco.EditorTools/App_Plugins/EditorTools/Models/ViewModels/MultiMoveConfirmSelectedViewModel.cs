using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels
{
    public class MultiMoveConfirmSelectedViewModel
    {
        public Dictionary<int,string> Selected { get; set; }
        public List<MultiMoveContentModel> Content { get; set; }

        public MultiMoveConfirmSelectedViewModel()
        {
            Selected = new Dictionary<int, string>();
        }
    }
}