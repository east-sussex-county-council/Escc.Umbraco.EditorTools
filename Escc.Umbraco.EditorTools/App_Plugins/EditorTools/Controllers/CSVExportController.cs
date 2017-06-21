using System;
using System.Text;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class CSVExportController : UmbracoAuthorizedController
    {
        public ActionResult Index()
        {
            return View("~/App_Plugins/EditorTools/Views/CSVExport/Index.cshtml");
        }

        public void GetFile()
        {
            // instantiate a string builder for our csv file
            var sb = new StringBuilder();

            // append the first row of column names
            sb.Append(string.Format("{0},{1},{2},{3},{4},{5}", "Header", "Template", "Document Type", "Expiry Date", "Edit Url", "Live Url") + Environment.NewLine);

            // get all nodes at the root of the content tree
            var root = ApplicationContext.Services.ContentService.GetRootContent();
            foreach (var node in root)
            {
                // get the descendants of the node
                var descendants = ApplicationContext.Services.ContentService.GetDescendants(node.Id);

                // append the node to the stringbuilder
                AppendToBuilder(sb, node);

                foreach (var child in descendants)
                {
                    //append the child to the stringbuilder
                    AppendToBuilder(sb, child);
                }
            }

            // When not using an API controller to return a HTTPResonseMessage we must write directly to the HttpContext.Current.Response to return the file.
            string attachment = "attachment; filename=ContentExport.csv";
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.ClearHeaders();
            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.AddHeader("content-disposition", attachment);
            System.Web.HttpContext.Current.Response.ContentType = "text/csv";
            System.Web.HttpContext.Current.Response.AddHeader("Pragma", "public");
            System.Web.HttpContext.Current.Response.Write(sb.ToString());
        }

        public void AppendToBuilder(StringBuilder sb, global::Umbraco.Core.Models.IContent node)
        {
            // get the node from the content cache
            var contentCacheNode = UmbracoContext.ContentCache.GetById(node.Id);

            // set variables to empty strings
            var name = "";
            var template = "";
            var expiryDate = "";
            var liveURL = "";
            var docType = "";

            // perform null checks on the fields
            if (node.Name == null) name = "Null";
            else name = node.Name;

            if (node.Template == null) template = "No Template Found";
            else template = node.Template.Name.ToString();

            if (node.ExpireDate == null) expiryDate = "No Expiry Date";
            else expiryDate = node.ExpireDate.ToString();

            if (contentCacheNode == null) liveURL = "Unpublished";
            else liveURL = contentCacheNode.Url;

            if (node.ContentType == null) docType = "None Found";
            else docType = node.ContentType.Name;

            // if the name contains a comma, surround it in "" so the csv doesnt treat each comma as the start of a new column
            if (name.Contains(","))
            {
                var temp = name;
                name = "\"" + temp.Replace("\"", "\"\"") + "\"";
            }

            // append to the string builder
            sb.Append(string.Format("{0},{1},{2},{3},{4},{5}", name, template, docType, expiryDate, "/umbraco#/content/content/edit/" + node.Id, liveURL) + Environment.NewLine);
        }
    }
}