namespace AestusDemoAPI.Domain.Dtos
{
    public record DailySuspiciousSummaryDto
    {
        public string UserId { get; set; } = string.Empty;
        public int Count { get; set; }
        public double TotalAmount { get; set; }
    }
}
