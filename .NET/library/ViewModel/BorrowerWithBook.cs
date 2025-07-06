using OneBeyondApi.Model;
using System.Text.Json.Serialization;

namespace OneBeyondApi.ViewModel
{
    public class BorrowerWithBook
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public virtual List<Book> Books { get; set; } = new List<Book>();
    }
}
