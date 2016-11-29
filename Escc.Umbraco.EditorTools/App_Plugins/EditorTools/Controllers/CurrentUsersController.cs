using ApiTest.Models;
using System.Collections.Generic;
using Umbraco.Web.WebApi;

namespace umbracoTools.App_Plugins.EditorTools.Controllers
{
    // Controller For current users node, (To Be Implemented)
    public class CurrentUsersController : UmbracoAuthorizedApiController
    {
        /* List<User> userList = new List<User>();

         public IEnumerable<User> GetAllUsers()
         {
             var userService = ApplicationContext.Services.UserService;
             int totalRecords;
             foreach (var user in userService.GetAll(0, 100, out totalRecords))
             {
                 userList.Add(new User(user.Id, user.Name, user.Username));
             }
             return userList;
         }*/
    } 
}