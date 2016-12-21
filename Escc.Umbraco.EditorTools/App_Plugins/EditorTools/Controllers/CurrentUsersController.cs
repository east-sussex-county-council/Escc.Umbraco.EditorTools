using Escc.Umbraco.EditorTools.Models;
using System.Collections.Generic;
using Umbraco.Web.WebApi;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Controllers
{
    // Controller For current users node, (To Be Implemented)
    public class CurrentUsersController : UmbracoAuthorizedApiController
    {
        // create a list to store users
         List<User> userList = new List<User>();
         List<User> disabledUserList = new List<User>();
        public IEnumerable<User> GetAllUsers()
         {
            // initialize the user service
             var userService = ApplicationContext.Services.UserService;
             int totalRecords;
            // for each user in the user service
             foreach (var user in userService.GetAll(0, int.MaxValue, out totalRecords))
             {
                if(user.IsApproved == true)
                {
                    // add the users id, name , username, user type and email to the list
                    userList.Add(new User(user.Id, user.Name, user.Username, user.UserType.Alias, user.Email));
                }
             }
             return userList;
         }

        public IEnumerable<User> GetAllDisabledUsers()
        {
            // initialize the user service
            var userService = ApplicationContext.Services.UserService;
            int totalRecords;
            // for each user in the user service
            foreach (var user in userService.GetAll(0, int.MaxValue, out totalRecords))
            {
                // add the users id, name , username, user type and email to the list
                if (user.IsApproved == false)
                {
                    disabledUserList.Add(new User(user.Id, user.Name, user.Username, user.UserType.Alias, user.Email));
                }
            }
            return disabledUserList;
        }

        public void GetEnableUser(string username)
        {
            // initialize the user service
            var userService = ApplicationContext.Services.UserService;
            // get the user model by username
            var user = userService.GetByUsername(username);
            // Reenable the user and save the model
            user.IsLockedOut = false;
            user.IsApproved = true;
            userService.Save(user, false);
        }

        public void GetDisableUser(string username)
        {
            // initialize the user service
            var userService = ApplicationContext.Services.UserService;
            // get the user model by username
            var user = userService.GetByUsername(username);
            // Disable the user and save the model
            user.IsLockedOut = true;
            user.IsApproved = false;
            userService.Save(user, false);
        }
    } 
}