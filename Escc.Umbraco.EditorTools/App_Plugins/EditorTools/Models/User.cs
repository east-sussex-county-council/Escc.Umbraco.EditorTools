using System;
using Umbraco.Core.Models.Membership;

namespace Escc.Umbraco.EditorTools.Models
{
    // Object to store the Name of a User
    public class User
    {
        public string UserType { get; set; }
        public string Email { get; set; }

        public int Id { get; set; }
        public string UserName { get; set; }

        public string Name { get; set; }

        public User(int id, string name, string username, string userType, string email)
        {
            Id = id;
            Name = name;
            UserName = username;
            UserType = userType;
            Email = email;
        }
    }
}