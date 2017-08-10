using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels
{
    public class MultiMoveContentModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
        public int Level { get; set; }
        public string Description { get; set; }
        public List<int> Children { get; set; }


        public MultiMoveContentModel(int id, string name, int parentID, int level)
        {
            ID = id;
            Name = name;
            ParentID = parentID;
            Level = level;
            Children = new List<int>();
        }

        public MultiMoveContentModel()
        {

        }
    }
}