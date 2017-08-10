using System;
using System.Collections.Generic;
using umbraco.businesslogic;
using umbraco.BusinessLogic.Actions;
using umbraco.cms.presentation.Trees;
using umbraco.interfaces;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools
{
    [Tree("EditorTools", "EditorToolsTree", "Example")]
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
            var IndexNode = XmlTreeNode.Create(this);
            IndexNode.NodeID = "0";
            IndexNode.NodeType = "Home";
            IndexNode.Text = "Home";
            IndexNode.Action = "javascript:openPage('/umbraco/backoffice/Plugins/EditorToolsHome/Index');";
            IndexNode.Icon = "icon-home";
            IndexNode.HasChildren = false;
            IndexNode.Menu = new List<IAction>();
            OnBeforeNodeRender(ref tree, ref IndexNode, EventArgs.Empty);
            if (IndexNode != null)
            {
                tree.Add(IndexNode);
                OnAfterNodeRender(ref tree, ref IndexNode, EventArgs.Empty);
            }

            var UsersNode = XmlTreeNode.Create(this);
            UsersNode.NodeID = "1";
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
            ContentToolsNode.NodeID = "2";
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
            MediaToolsNode.NodeID = "3";
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
            PageExpiryNode.NodeID = "4";
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

            var ExamineSearchNode = XmlTreeNode.Create(this);
            ExamineSearchNode.NodeID = "5";
            ExamineSearchNode.NodeType = "ExamineSearch";
            ExamineSearchNode.Text = "Examine Search";
            ExamineSearchNode.Action = "javascript:openPage('/umbraco/backoffice/Plugins/ExamineSearch/Index');";
            ExamineSearchNode.Icon = "icon-search";
            ExamineSearchNode.HasChildren = false;
            ExamineSearchNode.Menu = new List<IAction>();
            OnBeforeNodeRender(ref tree, ref ExamineSearchNode, EventArgs.Empty);
            if (ExamineSearchNode != null)
            {
                tree.Add(ExamineSearchNode);
                OnAfterNodeRender(ref tree, ref ExamineSearchNode, EventArgs.Empty);
            }

            var CSVExportNode = XmlTreeNode.Create(this);
            CSVExportNode.NodeID = "6";
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

            var InBoundLinkCheckerNode = XmlTreeNode.Create(this);
            InBoundLinkCheckerNode.NodeID = "7";
            InBoundLinkCheckerNode.NodeType = "InBoundLinkChecker";
            InBoundLinkCheckerNode.Text = "What Links Where?";
            InBoundLinkCheckerNode.Action = "javascript:openPage('/umbraco/backoffice/Plugins/InBoundLinkChecker/Index');";
            InBoundLinkCheckerNode.Icon = "icon-return-to-top";
            InBoundLinkCheckerNode.HasChildren = false;
            InBoundLinkCheckerNode.Menu = new List<IAction>();
            OnBeforeNodeRender(ref tree, ref InBoundLinkCheckerNode, EventArgs.Empty);
            if (InBoundLinkCheckerNode != null)
            {
                tree.Add(InBoundLinkCheckerNode);
                OnAfterNodeRender(ref tree, ref InBoundLinkCheckerNode, EventArgs.Empty);
            }

            var StatsNode = XmlTreeNode.Create(this);
            StatsNode.NodeID = "8";
            StatsNode.NodeType = "Stats";
            StatsNode.Text = "Stats";
            StatsNode.Action = "javascript:openPage('/umbraco/backoffice/Plugins/Stats/Index');";
            StatsNode.Icon = "icon-info";
            StatsNode.HasChildren = false;
            StatsNode.Menu = new List<IAction>();
            OnBeforeNodeRender(ref tree, ref StatsNode, EventArgs.Empty);
            if (StatsNode != null)
            {
                tree.Add(StatsNode);
                OnAfterNodeRender(ref tree, ref StatsNode, EventArgs.Empty);
            }

            var MultiMoveNode = XmlTreeNode.Create(this);
            MultiMoveNode.NodeID = "9";
            MultiMoveNode.NodeType = "MultiMove";
            MultiMoveNode.Text = "Multi Move";
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