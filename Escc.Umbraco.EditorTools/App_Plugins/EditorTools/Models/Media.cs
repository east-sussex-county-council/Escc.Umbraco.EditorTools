using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models
{
    public class Media
    {
        public string ContentType { get; set; }
        public string CreateDate { get; set; }
        public string Name { get; set; }
        public string CreatorName { get; set; }

        public int Id { get; set; }

        public Media(string name, string contentType, string createDate, int id, string creatorName)
        {
            Name = name;
            ContentType = contentType;
            CreateDate = createDate;
            Id = id;
            CreatorName = creatorName;
        }
    }
}