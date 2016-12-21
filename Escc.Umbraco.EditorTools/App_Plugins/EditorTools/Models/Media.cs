﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models
{
    public class Media
    {
        public string CreateDate { get; set; }
        public string Name { get; set; }
        public string CreatorName { get; set; }
        public string Url { get; set; }

        public int Id { get; set; }

        public Media(string name, string createDate, int id, string creatorName, string url)
        {
            Name = name;
            CreateDate = createDate;
            Id = id;
            CreatorName = creatorName;
            Url = url;
        }
    }
}