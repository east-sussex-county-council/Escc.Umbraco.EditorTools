using System;

namespace Escc.Umbraco.EditorTools.Models
{
    // Object to store the Url, Id, Name and DocumentTypeAlies of a content Node
    public class Content
    {
        public string Url { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string DocumentTypeAlias { get; set; }

        public Content(int id, string name, string documentTypeAlias, string url)
        {
            Id = id;
            Name = name;
            this.DocumentTypeAlias = documentTypeAlias;
            this.Url = url;
        }
    }
}