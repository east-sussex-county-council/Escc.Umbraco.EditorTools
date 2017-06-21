using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels
{
    public class UsersViewModel
    {
        public TableModel ActiveUsers { get; set; }
        public TableModel DisabledUsers { get; set; }

        public UsersViewModel()
        {
            ActiveUsers = new TableModel("ActiveUsersTable");
            DisabledUsers = new TableModel("DisabledUsersTable");
        }
    }
}