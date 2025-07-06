namespace OneBeyondApi.Model
{
    public class Fine
    {
        public Guid Id { get; set; }
        public required Decimal Amount { get; set; }
        public required DateTime OriginalExpiration { get; set; }
        public required DateTime ReturnedAt { get; set; }
        public required Borrower Borrower { get; set; }

    }
}
