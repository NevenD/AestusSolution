using AestusDemoAPI.Domain;
using AestusDemoAPI.Domain.Dtos;
using AestusDemoAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AestusDemoAPI.EndpointHandlers
{
    public sealed class DashboardHandler
    {
        public static async Task<IResult> GetDashboardData(FinTechAestusContext db)
        {
            var allTransactions = await db.Transactions
                .AsNoTracking()
                .Select(t => t.ToTransactionDto())
                .ToListAsync();

            var suspiciousTransactions = allTransactions
                .Where(t => t.IsSuspicious)
                .ToList();

            var totalAmount = allTransactions.Sum(t => t.Amount);

            var summary = new DashboardDataDto
            {
                TotalTransactions = allTransactions.Count,
                TotalAmount = Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero),
                SuspiciousTransactionsCount = suspiciousTransactions.Count,
                Transactions = allTransactions,
                DailySuspiciousSummary = allTransactions
                .GroupBy(t => t.UserId)
                .Select(g => new DailySuspiciousSummaryDto
                {
                    UserId = g.Key,
                    Count = g.Count(),
                    TotalAmount = g.Sum(t => t.Amount)
                }).ToList()
            };

            return Results.Ok(summary);
        }

    }
}
