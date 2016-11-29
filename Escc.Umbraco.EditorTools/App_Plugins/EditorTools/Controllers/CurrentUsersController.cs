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

         public IEnumerable<User> GetAllUsers()
         {
            // initialize the user service
             var userService = ApplicationContext.Services.UserService;
             int totalRecords;
            // for each user in the user service
             foreach (var user in userService.GetAll(0, int.MaxValue, out totalRecords))
             {
                // add the users id, name , username, user type and email to the list
                 userList.Add(new User(user.Id, user.Name, user.Username, user.UserType.Alias, user.Email));
             }
             return userList;
         }
    } 
}