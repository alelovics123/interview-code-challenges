using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public interface ICatalogueRepository
    {
        public List<BookStock> GetCatalogue();
        public void ReturnLoan(Guid bookStockId, Decimal fineAmount);
        public string GetLoanableDate(Guid bookId, int daysPerfutureReservation);
        public List<BookStock> SearchCatalogue(CatalogueSearch search);
        public void Reserve(Guid bookId, Guid borrowerId, int reservationTimeInDays);
    }
}
