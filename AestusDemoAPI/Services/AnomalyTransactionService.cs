using AestusDemoAPI.Domain.Entitites;
using AestusDemoAPI.Infrastructure;
using AestusDemoAPI.Validation;
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

            if (TransactionAnomalyRules.IsInvalidAmount(transaction))
            {
                return true;
            }

            if (TransactionAnomalyRules.IsFrequencySpike(recentTransactions))
            {
                return true;
            }

            if (TransactionAnomalyRules.IsIQRAnomaly(transaction, recentTransactions))
            {
                return true;
            }

            if (TransactionAnomalyRules.IsZScoreAnomaly(transaction, recentTransactions))
            {
                return true;
            }

            return false;
        }
    }
}
