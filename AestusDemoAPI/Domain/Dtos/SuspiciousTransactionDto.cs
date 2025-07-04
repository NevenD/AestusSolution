namespace AestusDemoAPI.Domain.Dtos
{
    public record SuspiciousTransactionDto
    {
        public string UserId { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string AmountWithCurrency { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
    }
}
