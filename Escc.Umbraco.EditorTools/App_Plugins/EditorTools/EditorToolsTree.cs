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
            IndexNode.Action = "javascript:openPage('/umbraco/backoffice/Plugins/Home/Index');";
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

            var DocumentTypesNode = XmlTreeNode.Create(this);
            DocumentTypesNode.NodeID = "2";
            DocumentTypesNode.NodeType = "DocumentTypes";
            DocumentTypesNode.Text = "Document Types";
            DocumentTypesNode.Action = "javascript:openPage('/umbraco/backoffice/Plugins/DocumentTypes/Index');";
            DocumentTypesNode.Icon = "icon-document";
            DocumentTypesNode.HasChildren = false;
            DocumentTypesNode.Menu = new List<IAction>();
            OnBeforeNodeRender(ref tree, ref DocumentTypesNode, EventArgs.Empty);
            if (DocumentTypesNode != null)
            {
                tree.Add(DocumentTypesNode);
                OnAfterNodeRender(ref tree, ref DocumentTypesNode, EventArgs.Empty);
            }

            var CSVExportNode = XmlTreeNode.Create(this);
            CSVExportNode.NodeID = "3";
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

            var ExamineSearchNode = XmlTreeNode.Create(this);
            ExamineSearchNode.NodeID = "4";
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

            var PageExpiryNode = XmlTreeNode.Create(this);
            PageExpiryNode.NodeID = "5";
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