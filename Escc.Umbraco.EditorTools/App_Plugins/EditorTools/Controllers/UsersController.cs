using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    public class UsersController : UmbracoAuthorizedController
    {
        public ActionResult Index()
        {
            // instantiate the view model
            var model = new UsersViewModel();

            // populate the view models variables
            model.ActiveUsers.Table = CreateTable(true);
            model.DisabledUsers.Table = CreateTable(false);

            // return a view and pass it the view model.
            return View("~/App_Plugins/EditorTools/Views/Users/Index.cshtml", model);
        }

        public DataTable CreateTable(bool Active)
        {
            var userService = ApplicationContext.Services.UserService;

            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Username", typeof(string));
            table.Columns.Add("User Type", typeof(string));
            table.Columns.Add("Email", typeof(string));

            int totalRecords;
            // for each user in the user service
            foreach (var user in userService.GetAll(0, int.MaxValue, out totalRecords))
            {
                if (user.IsApproved == Active)
                {
                    // add the users id, name , username, user type and email to the table
                    table.Rows.Add(user.Id, user.Name, user.Username, user.UserType.Alias, user.Email);
                }
            }
            return table;
        }

    }
}