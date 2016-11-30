using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.EditorTools.Models
{
    public class NonPerishableContent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public NonPerishableContent(int id, string name, string url)
        {
            Id = id;
            Name = name;
            Url = url;
        }
    }
}