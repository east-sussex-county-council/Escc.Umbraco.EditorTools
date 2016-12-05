using System.Net.Http.Formatting;
using umbraco.BusinessLogic.Actions;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    [Tree("EditorTools", "EditorToolsTree", "Editor Tools")]
    [PluginController("EditorTools")]
    public class EditorToolsTreeController : TreeController
    {
        // Create the navigation tree listed under the EditorTools Section
        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            // Create a menu collection
            var menu = new MenuItemCollection();
            menu.DefaultMenuAlias = ActionNew.Instance.Alias;
            menu.Items.Add<ActionNew>("Create");
            return menu;  
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            // add nodes to the menu 
            var nodes = new TreeNodeCollection();
            string route = "";
            if (id == "-1") // root ensures all nodes are listed at the root of the menu
            {
                // route defines the uri for a node, linking a node to its view file
                route = string.Format("/EditorTools/EditorToolsTree/documentTypeUsageView/documentTypeUsage", "documentTypeUsage");
                nodes.Add(CreateTreeNode("documentTypeUsage", id, queryStrings, "Document Type Usage", "icon-document", false, route));

                route = string.Format("/EditorTools/EditorToolsTree/currentUsersView/currentUsers", "currentUsers");
                nodes.Add(CreateTreeNode("currentUsers", id, queryStrings, "Current Users", "icon-users", false, route));

                route = string.Format("/EditorTools/EditorToolsTree/pageExpiryReportView/pageExpiryReport", "pageExpiryReport");
                nodes.Add(CreateTreeNode("pageExpiryReport", id, queryStrings, "Page Expiry Report", "icon-timer", false, route));

                route = string.Format("/EditorTools/EditorToolsTree/exportToCSVView/exportToCSV", "exportToCSV");
                nodes.Add(CreateTreeNode("exportToCSV", id, queryStrings, "Export To CSV", "icon-download-alt", false, route));

                route = string.Format("/EditorTools/EditorToolsTree/mediaSearchView/mediaSearch", "exportToCSV");
                nodes.Add(CreateTreeNode("mediaSearch", id, queryStrings, "Media Search", "icon-file-cabinet", false, route));
            }
            return nodes;
        }
    }
}
