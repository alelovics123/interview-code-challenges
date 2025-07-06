using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public interface IBorrowerRepository
    {
        public List<Borrower> GetBorrowers();
        public List<Borrower> GetOnLoan();

        public Guid AddBorrower(Borrower borrower);
    }
}
