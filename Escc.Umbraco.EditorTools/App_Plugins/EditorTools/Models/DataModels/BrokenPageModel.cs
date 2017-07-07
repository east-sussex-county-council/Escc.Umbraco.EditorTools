using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels
{
    public class BrokenPageModel
    {
        public string URL { get; set; }
        public string FoundOn { get; set; }
        public string Exception { get; set; }

        public BrokenPageModel(string url, string foundOn, string exception)
        {
            URL = url;
            FoundOn = foundOn;
            Exception = exception;
        }
    }
}