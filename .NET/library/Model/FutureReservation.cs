namespace OneBeyondApi.Model
{
    public class FutureReservation
    {
        public Guid Id { get; set; }
        public required Book Book { get; set; }
        public required Borrower Borrower{get;set;}
        public required int Order {  get; set; }
    }
}
