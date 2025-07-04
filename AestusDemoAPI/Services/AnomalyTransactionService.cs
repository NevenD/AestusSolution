using AestusDemoAPI.Domain.Entitites;
using AestusDemoAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AestusDemoAPI.Services
{
    public interface IAnomalyDetectionService
    {
        Task<bool> CheckAsync(Transaction transaction, FinTechAestusContext db);
    }

    public class AnomalyTransactionService : IAnomalyDetectionService
    {
        public async Task<bool> CheckAsync(Transaction transaction, FinTechAestusContext db)
        {
            var recentTransactions = await db.Transactions
               .Where(t => t.UserId == transaction.UserId && t.Timestamp < transaction.Timestamp)
               .OrderByDescending(t => t.Timestamp)
               .Take(1000)
               .ToListAsync();

            if (IsExtremeAmount(transaction))
            {
                return true;
            }

            if (IsFrequencySpike(transaction, recentTransactions))
            {
                return true;
            }

            if (IsIQRAnomaly(transaction, recentTransactions))
            {
                return true;
            }

            if (IsZScoreAnomaly(transaction, recentTransactions))
            {
                return true;
            }

            return false;
        }

        private static bool IsExtremeAmount(Transaction transaction)
        {
            return transaction.Amount > 100_000;
        }

        private static bool IsFrequencySpike(Transaction transaction, List<Transaction> recentTransactions)
        {
            var fiveMinutesAgo = DateTime.UtcNow.AddMinutes(-5);
            var recentCount = recentTransactions.Count(t => t.Timestamp > fiveMinutesAgo);
            return recentCount > 10;
        }

        // Represents Interquartile Range
        private static bool IsIQRAnomaly(Transaction tx, List<Transaction> txs)
        {
            if (txs.Count < 10) return false;

            var sorted = txs.Select(t => t.Amount).OrderBy(a => a).ToList();
            var q1 = sorted[sorted.Count / 4];
            var q3 = sorted[3 * sorted.Count / 4];
            var iqr = q3 - q1;

            var lower = q1 - (1.5 * iqr);
            var upper = q3 + (1.5 * iqr);

            return tx.Amount < lower || tx.Amount > upper;
        }

        private static bool IsZScoreAnomaly(Transaction tx, List<Transaction> txs)
        {
            if (txs.Count < 10)
            {
                return false;
            }

            var amounts = txs.Select(t => t.Amount).ToList();
            var mean = amounts.Average();
            var stdDev = Math.Sqrt(amounts.Average(a => Math.Pow((double)(a - mean), 2)));

            if (stdDev == 0)
            {
                return false;
            }

            var z = (double)(tx.Amount - mean) / stdDev;
            return Math.Abs(z) > 3; // Z-score threshold
        }
    }
}
