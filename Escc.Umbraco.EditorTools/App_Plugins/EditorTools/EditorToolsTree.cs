using System;
using System.Collections.Generic;
using umbraco.businesslogic;
using umbraco.BusinessLogic.Actions;
using umbraco.cms.presentation.Trees;
using umbraco.interfaces;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools
{
    [Tree("EditorTools", "EditorToolsTree", "Editor Tools")]
    public class EditorToolsTree : BaseTree
    {
        public EditorToolsTree(string application)
            : base(application)
        {
        }

        protected override void CreateRootNode(ref XmlTreeNode rootNode)
        {
            rootNode.NodeType = "example";
            rootNode.NodeID = "init";
            rootNode.Menu = new List<IAction> { ActionRefresh.Instance };
        }

        public override void Render(ref XmlTree tree)
        {
            var UsersNode = XmlTreeNode.Create(this);
            UsersNode.NodeID = "0";
            UsersNode.NodeType = "Users";
            UsersNode.Text = "Users";
            UsersNode.Action = "javascript:openPage('/umbraco/backoffice/Plugins/Users/Index');";
            UsersNode.Icon = "icon-users";
            UsersNode.HasChildren = false;
            UsersNode.Menu = new List<IAction>();
            OnBeforeNodeRender(ref tree, ref UsersNode, EventArgs.Empty);
            if (UsersNode != null)
            {
                tree.Add(UsersNode);
                OnAfterNodeRender(ref tree, ref UsersNode, EventArgs.Empty);
            }

            var ContentToolsNode = XmlTreeNode.Create(this);
            ContentToolsNode.NodeID = "1";
            ContentToolsNode.NodeType = "ContentTools";
            ContentToolsNode.Text = "Content";
            ContentToolsNode.Action = "javascript:openPage('/umbraco/backoffice/Plugins/Content/Index');";
            ContentToolsNode.Icon = "icon-document";
            ContentToolsNode.HasChildren = false;
            ContentToolsNode.Menu = new List<IAction>();
            OnBeforeNodeRender(ref tree, ref ContentToolsNode, EventArgs.Empty);
            if (ContentToolsNode != null)
            {
                tree.Add(ContentToolsNode);
                OnAfterNodeRender(ref tree, ref ContentToolsNode, EventArgs.Empty);
            }

            var MediaToolsNode = XmlTreeNode.Create(this);
            MediaToolsNode.NodeID = "2";
            MediaToolsNode.NodeType = "MediaTools";
            MediaToolsNode.Text = "Media";
            MediaToolsNode.Action = "javascript:openPage('/umbraco/backoffice/Plugins/Media/Index');";
            MediaToolsNode.Icon = "icon-umb-media";
            MediaToolsNode.HasChildren = false;
            MediaToolsNode.Menu = new List<IAction>();
            OnBeforeNodeRender(ref tree, ref MediaToolsNode, EventArgs.Empty);
            if (MediaToolsNode != null)
            {
                tree.Add(MediaToolsNode);
                OnAfterNodeRender(ref tree, ref MediaToolsNode, EventArgs.Empty);
            }

            var PageExpiryNode = XmlTreeNode.Create(this);
            PageExpiryNode.NodeID = "3";
            PageExpiryNode.NodeType = "PageExpiry";
            PageExpiryNode.Text = "Page Expiry";
            PageExpiryNode.Action = "javascript:openPage('/umbraco/backoffice/Plugins/PageExpiry/Index');";
            PageExpiryNode.Icon = "icon-timer";
            PageExpiryNode.HasChildren = false;
            PageExpiryNode.Menu = new List<IAction>();
            OnBeforeNodeRender(ref tree, ref PageExpiryNode, EventArgs.Empty);
            if (PageExpiryNode != null)
            {
                tree.Add(PageExpiryNode);
                OnAfterNodeRender(ref tree, ref PageExpiryNode, EventArgs.Empty);
            }

            var ExpiryBulkUpdateNode = XmlTreeNode.Create(this);
            ExpiryBulkUpdateNode.NodeID = "4";
            ExpiryBulkUpdateNode.NodeType = "ExpiryBulkUpdate";
            ExpiryBulkUpdateNode.Text = "Bulk Update Expiry Dates";
            ExpiryBulkUpdateNode.Action = "javascript:openPage('/umbraco/backoffice/Plugins/ExpiryBulkUpdate/Index');";
            ExpiryBulkUpdateNode.Icon = "icon-timer";
            ExpiryBulkUpdateNode.HasChildren = false;
            ExpiryBulkUpdateNode.Menu = new List<IAction>();
            OnBeforeNodeRender(ref tree, ref ExpiryBulkUpdateNode, EventArgs.Empty);
            if (ExpiryBulkUpdateNode != null)
            {
                tree.Add(ExpiryBulkUpdateNode);
                OnAfterNodeRender(ref tree, ref ExpiryBulkUpdateNode, EventArgs.Empty);
            }

            var CSVExportNode = XmlTreeNode.Create(this);
            CSVExportNode.NodeID = "5";
            CSVExportNode.NodeType = "CSVExport";
            CSVExportNode.Text = "CSV Export";
            CSVExportNode.Action = "javascript:openPage('/umbraco/backoffice/Plugins/CSVExport/Index');";
            CSVExportNode.Icon = "icon-download-alt";
            CSVExportNode.HasChildren = false;
            CSVExportNode.Menu = new List<IAction>();
            OnBeforeNodeRender(ref tree, ref CSVExportNode, EventArgs.Empty);
            if (CSVExportNode != null)
            {
                tree.Add(CSVExportNode);
                OnAfterNodeRender(ref tree, ref CSVExportNode, EventArgs.Empty);
            }

            var MultiMoveNode = XmlTreeNode.Create(this);
            MultiMoveNode.NodeID = "6";
            MultiMoveNode.NodeType = "MultiMove";
            MultiMoveNode.Text = "Multi Page Mover";
            MultiMoveNode.Action = "javascript:openPage('/umbraco/backoffice/Plugins/MultiMove/Index');";
            MultiMoveNode.Icon = "icon-nodes";
            MultiMoveNode.HasChildren = false;
            MultiMoveNode.Menu = new List<IAction>();
            OnBeforeNodeRender(ref tree, ref MultiMoveNode, EventArgs.Empty);
            if (MultiMoveNode != null)
            {
                tree.Add(MultiMoveNode);
                OnAfterNodeRender(ref tree, ref MultiMoveNode, EventArgs.Empty);
            }
        }

        public override void RenderJS(ref System.Text.StringBuilder Javascript)
        {
            Javascript.Append(
               @"function openPage(url) {
                 UmbClientMgr.contentFrame(url);
                }");
        }
    }
}