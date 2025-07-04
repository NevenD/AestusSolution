namespace AestusDemoAPI.Domain.Dtos
{
    public record DailySuspiciousSummaryDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
        public string AmountWithCurrency { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
    }
}
