using System;

namespace ApiTest.Models
{
    // Object to store the Name of a User
    public class User
    {
        public string Name { get; set; } 

        public User(string name)
        {
            Name = name;
        }
    }
}