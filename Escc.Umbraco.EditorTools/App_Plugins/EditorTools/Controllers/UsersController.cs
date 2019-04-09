using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class UsersController : UmbracoAuthorizedController
    {
        private MemoryCache cache = MemoryCache.Default;

        public ActionResult Index()
        {
            var model = cache["UsersViewModel"] as UsersViewModel;

            if (model == null)
            {
                model = new UsersViewModel();
                model.CachedDataAvailable = false;
            }

            // return a view and pass it the view model.
            return View("~/App_Plugins/EditorTools/Views/Users/Index.cshtml", model);
        }

        #region Helpers
        public UsersViewModel CreateModel()
        {
            var model = new UsersViewModel();
            var userService = ApplicationContext.Services.UserService;

            model.ActiveUsers.Table = new DataTable();
            model.ActiveUsers.Table.Columns.Add("ID", typeof(int));
            model.ActiveUsers.Table.Columns.Add("Name", typeof(string));
            model.ActiveUsers.Table.Columns.Add("Username", typeof(string));
            model.ActiveUsers.Table.Columns.Add("Sections", typeof(string));
            model.ActiveUsers.Table.Columns.Add("Email", typeof(string));

            model.DisabledUsers.Table = new DataTable();
            model.DisabledUsers.Table.Columns.Add("ID", typeof(int));
            model.DisabledUsers.Table.Columns.Add("Name", typeof(string));
            model.DisabledUsers.Table.Columns.Add("Username", typeof(string));
            model.DisabledUsers.Table.Columns.Add("Sections", typeof(string));
            model.DisabledUsers.Table.Columns.Add("Email", typeof(string));

            var umbracoVersionSupportsGroups = double.Parse(ConfigurationManager.AppSettings["UmbracoConfigurationStatus"].Substring(0, ConfigurationManager.AppSettings["UmbracoConfigurationStatus"].LastIndexOf("."))) >= 7.7;

            int totalRecords;
            // for each user in the user service
            foreach (var user in userService.GetAll(0, int.MaxValue, out totalRecords))
            {
                var editURL = new HtmlString(umbracoVersionSupportsGroups ? 
                    $"<a target=\"_top\" href=\"/umbraco#/users/users/user/{user.Id}?subview=users\">{user.Name}</a>" : 
                    $"<a target=\"_top\" href=\"/umbraco#/users/framed/%252Fumbraco%252Fusers%252FeditUser.aspx%253Fid%253D{user.Id}\">{user.Name}</a>");
                if (user.IsApproved)
                {
                    model.ActiveUsers.Table.Rows.Add(user.Id, editURL, user.Username, string.Join(", ", user.AllowedSections.ToArray()), user.Email);
                }
                else
                {
                    model.DisabledUsers.Table.Rows.Add(user.Id, editURL, user.Username, string.Join(", ", user.AllowedSections.ToArray()), user.Email);
                }
            }

            return model;
        }
        #endregion

        #region Cache Methods
        private void StoreInCache(UsersViewModel model)
        {
            if (cache.Contains("UsersViewModel"))
            {
                cache.Remove("UsersViewModel");
            }
            cache.Add("UsersViewModel", model, DateTime.Now.AddHours(1));
        }

        public ActionResult RefreshCache()
        {
            // instantiate the view model
            var model = CreateModel();
            StoreInCache(model);
            return View("~/App_Plugins/EditorTools/Views/Users/Index.cshtml", model);
        }
        #endregion

    }
}