using OneBeyondApi.Model;
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

        public List<Borrower> GetOnLoan()
        {
            using (var context = new LibraryContext())
            {
                var list = context.Borrowers.Where(n=>n.BookStocks.Any(m=>m.LoanEndDate.GetValueOrDefault()<DateTime.Now))   
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
