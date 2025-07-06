namespace AestusDemoAPI.Domain.Dtos
{
    public record TransactionDto
    {
        public string UserId { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string AmountWithCurrency { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string FormatedDate { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsSuspicious { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
