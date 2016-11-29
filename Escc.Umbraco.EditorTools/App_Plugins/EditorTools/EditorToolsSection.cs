using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.businesslogic;
using umbraco.interfaces;

namespace umbracoTools.App_Plugins.EditorTools
{
    // This class creates a new section in umbraco
    [Application("EditorTools", "EditorTools", "icon-tools", 8)]
    public class EditorToolsSection : IApplication
    { }
}