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
        public DateTime CacheDate { get; set; }
        public TableModel UserTypes { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsersCount { get; set; }
        public int DisabledUsersCount { get; set; }

        public UsersViewModel()
        {
            ActiveUsers = new TableModel("ActiveUsersTable");
            DisabledUsers = new TableModel("DisabledUsersTable");
            UserTypes = new TableModel("UserTypesTable");
            CacheDate = DateTime.Now;
        }
    }
}