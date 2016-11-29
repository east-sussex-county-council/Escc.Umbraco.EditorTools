namespace ApiTest.Models
{
    public class DocumentType
    {
        // object to store the id, name and count for a DocumentTyoe
        public DocumentType(int id, string name, int count)
        {
            this.Id = id;
            this.Name = name;
            this.Count = count;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }

    }
}