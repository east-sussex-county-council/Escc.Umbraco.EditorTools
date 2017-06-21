using Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.DataModels;
using System.Collections.Generic;

namespace Escc.Umbraco.EditorTools.App_Plugins.EditorTools.Models.ViewModels
{
    public class DocumentTypesViewModel
    {
        public TableModel DocumentTypes { get;set;}
        public Dictionary<string, TableModel> ModalTables { get; set; }

        public DocumentTypesViewModel()
        {
            ModalTables = new Dictionary<string, TableModel>();
            DocumentTypes = new TableModel("DocumentsOfTypeTable");
        }
    }
}