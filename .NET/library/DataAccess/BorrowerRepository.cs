using Microsoft.EntityFrameworkCore;
using OneBeyondApi.Model;
using OneBeyondApi.ViewModel;
using System.Linq;

namespace OneBeyondApi.DataAccess
{
    public class BorrowerRepository : IBorrowerRepository
    {
        public BorrowerRepository()
        {
        }
        public List<Borrower> GetBorrowers()
        {
            using (var context = new LibraryContext())
            {
                var list = context.Borrowers
                    .ToList();
                return list;
            }
        }

        public List<BorrowerWithBook> GetOnLoan()
        {
            using (var context = new LibraryContext())
            {
                var list = context.Borrowers.Where(n => n.BookStocks.Any()).Include(n => n.BookStocks).ThenInclude(n => n.Book).ThenInclude(n=>n.Author)
                    .Select(n => new BorrowerWithBook 
                    { 
                        Books = n.BookStocks.Select(m => m.Book).ToList(),
                        EmailAddress = n.EmailAddress,
                        Id = n.Id,
                        Name = n.Name 
                    })
                    .ToList();
                return list;
            }
        }

        public Guid AddBorrower(Borrower borrower)
        {
            using (var context = new LibraryContext())
            {
                context.Borrowers.Add(borrower);
                context.SaveChanges();
                return borrower.Id;
            }
        }
    }
}
