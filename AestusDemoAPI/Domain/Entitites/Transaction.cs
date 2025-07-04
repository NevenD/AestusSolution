namespace AestusDemoAPI.Domain.Entitites
{
    public class Transaction
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public double Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public required string Location { get; set; }
        public bool IsSuspicious { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
