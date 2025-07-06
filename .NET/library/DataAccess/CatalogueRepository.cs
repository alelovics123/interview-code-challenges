using Microsoft.AspNetCore.Server.IIS;
using Microsoft.EntityFrameworkCore;
using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public class CatalogueRepository : ICatalogueRepository
    {
        public static object _lockObject=new object();
        public CatalogueRepository()
        {
        }
        public List<BookStock> GetCatalogue()
        {
            using (var context = new LibraryContext())
            {
                var list = context.Catalogue
                    .Include(x => x.Book)
                    .ThenInclude(x => x.Author)
                    .Include(x => x.OnLoanTo)
                    .ToList();
                return list;
            }
        }

        public string GetLoanableDate(Guid bookId,int daysPerFutureReservation)
        {
            using (var context = new LibraryContext())
            {
                var stocks = context.Catalogue.Where(n => n.Book.Id == bookId);
                if (!stocks.Any()) { return "No copies of the book"; }
                if (stocks.Any(m => m.OnLoanTo == null)) { return "Available"; }
                if (context.FutureReservations.All(m => m.Book.Id != bookId))
                {
                    if (stocks.Any(m => m.LoanEndDate.Value > DateTime.Now))
                    {
                        return "Availbable at"+stocks.Where(m => m.LoanEndDate.Value > DateTime.Now).Select(n => n.LoanEndDate).Min().ToString();
                    }
                    return "Indeterminable";
                }
                var futureReservationsCount = context.FutureReservations.Count(m => m.Book.Id == bookId);
                return futureReservationsCount.ToString() + " futre reservations. Copies of this book:" + stocks.Count()+". Likely available within"+Math.Ceiling(((double)futureReservationsCount*daysPerFutureReservation)/stocks.Count());
            }
        }

        public void Reserve(Guid bookId, Guid borrowerId, int reservationTimeInDays)
        {
            using (var context = new LibraryContext()) 
            {
                var book=context.Books.Find(bookId);
                var borrower=context.Borrowers.Find(borrowerId);
                
                if (book==null ||borrower==null ) 
                {
                    throw new Microsoft.AspNetCore.Http.BadHttpRequestException("Bad parameters");
                }
                lock (_lockObject) {
                    var stocks = context.Catalogue.Where(n => n.Book.Id == bookId);
                    if (!stocks.Any()) { throw new Exception("No stock"); }
                    if (stocks.Any(m=>m.OnLoanTo.Id==borrowerId)) { throw new Exception("Has borrowed already this book"); }
                    var unreservedStock = stocks.FirstOrDefault(n => n.LoanEndDate == null);
                    if (unreservedStock != null)
                    {
                        unreservedStock.LoanEndDate = DateTime.Now.AddDays(reservationTimeInDays);
                        unreservedStock.OnLoanTo = borrower;
                        var reservations=context.FutureReservations.Where(n => n.Borrower.Id == borrowerId);
                        if (reservations.Any())
                        {
                            context.RemoveRange(reservations);
                        }
                        context.Catalogue.Update(unreservedStock);
                        context.SaveChanges();
                        return;
                    }
                    else 
                    {
                        var order = context.FutureReservations.Where(n => n.Book.Id == bookId).Select(n => n.Order).DefaultIfEmpty(0).Max() + 1;
                        context.FutureReservations.Add(new FutureReservation { Book=book,Borrower=borrower,Order=order });
                        context.SaveChanges();
                    }
                }

            }
        }

        public void ReturnLoan(Guid bookStockId, Decimal fineAmount)
        {

            using (var context = new LibraryContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var loan = context.Catalogue.Find(bookStockId);
                        if (loan == null || loan.OnLoanTo != null) { throw new InvalidDataException(); }
                        if (loan.LoanEndDate < DateTime.Now)
                        {
                            context.Fines.Add(new Fine { Amount = fineAmount, Borrower = loan.OnLoanTo, OriginalExpiration = loan.LoanEndDate.Value, ReturnedAt = DateTime.UtcNow });

                        }
                        loan.OnLoanTo = null;
                        loan.LoanEndDate = null;
                        context.Catalogue.Update(loan);
                        context.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception ex) { transaction.Rollback(); throw; }
                }
            }
        }


        public List<BookStock> SearchCatalogue(CatalogueSearch search)
        {
            using (var context = new LibraryContext())
            {
                var list = context.Catalogue
                    .Include(x => x.Book)
                    .ThenInclude(x => x.Author)
                    .Include(x => x.OnLoanTo)
                    .AsQueryable();

                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.Author))
                    {
                        list = list.Where(x => x.Book.Author.Name.Contains(search.Author));
                    }
                    if (!string.IsNullOrEmpty(search.BookName))
                    {
                        list = list.Where(x => x.Book.Name.Contains(search.BookName));
                    }
                }

                return list.ToList();
            }
        }
    }
}
