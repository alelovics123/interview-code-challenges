using OneBeyondApi.Model;
using OneBeyondApi.ViewModel;

namespace OneBeyondApi.DataAccess
{
    public interface IBorrowerRepository
    {
        public List<Borrower> GetBorrowers();
        public List<BorrowerWithBook> GetOnLoan();

        public Guid AddBorrower(Borrower borrower);
    }
}
