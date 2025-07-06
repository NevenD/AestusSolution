namespace AestusDemoAPI.Domain.Dtos
{
    public record DashboardDataDto
    {
        public int TotalTransactions { get; set; }
        public double TotalAmount { get; set; }
        public int SuspiciousTransactionsCount { get; set; }
        public List<DailySuspiciousSummaryDto> DailySuspiciousSummary { get; set; } = [];
        public List<TransactionDto> Transactions { get; set; } = [];
    }
}
