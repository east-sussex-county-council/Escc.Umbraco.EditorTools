using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Caching;
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
            model.ActiveUsers.Table.Columns.Add("User Type", typeof(string));
            model.ActiveUsers.Table.Columns.Add("Email", typeof(string));

            model.DisabledUsers.Table = new DataTable();
            model.DisabledUsers.Table.Columns.Add("ID", typeof(int));
            model.DisabledUsers.Table.Columns.Add("Name", typeof(string));
            model.DisabledUsers.Table.Columns.Add("Username", typeof(string));
            model.DisabledUsers.Table.Columns.Add("User Type", typeof(string));
            model.DisabledUsers.Table.Columns.Add("Email", typeof(string));

            int totalRecords;
            // for each user in the user service
            foreach (var user in userService.GetAll(0, int.MaxValue, out totalRecords))
            {
                if (user.IsApproved)
                {
                    model.ActiveUsers.Table.Rows.Add(user.Id, user.Name, user.Username, user.UserType.Alias, user.Email);
                }
                else
                {
                    model.DisabledUsers.Table.Rows.Add(user.Id, user.Name, user.Username, user.UserType.Alias, user.Email);
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