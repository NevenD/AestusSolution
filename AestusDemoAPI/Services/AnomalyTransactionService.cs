using AestusDemoAPI.Domain.Dtos;
using AestusDemoAPI.Domain.Entitites;
using AestusDemoAPI.Infrastructure;
using AestusDemoAPI.Validation;
using Microsoft.EntityFrameworkCore;

namespace AestusDemoAPI.Services
{
    public interface IAnomalyDetectionService
    {
        Task<AnomalyStatusDto> CheckAsync(Transaction transaction, FinTechAestusContext db);

        AnomalyStatusDto CheckCached(Transaction transaction, List<Transaction> recentTransactions);
    }

    public class AnomalyTransactionService : IAnomalyDetectionService
    {

        public async Task<AnomalyStatusDto> CheckAsync(Transaction transaction, FinTechAestusContext db)
        {
            var recentTransactions = await db.Transactions
               .AsNoTracking()
               .Where(t => t.UserId == transaction.UserId && t.Timestamp < transaction.Timestamp)
               .Take(1000)
               .ToListAsync();

            return CheckCached(transaction, recentTransactions);
        }

        public AnomalyStatusDto CheckCached(Transaction transaction, List<Transaction> recentTransactions)
        {

            if (TransactionAnomalyRules.IsInvalidLocation(transaction))
            {
                return new AnomalyStatusDto { IsSuspicious = true, Comment = TransactionAnomalyMessages.UnexpectedLocation };
            }

            if (TransactionAnomalyRules.IsInvalidAmount(transaction))
            {
                return new AnomalyStatusDto { IsSuspicious = true, Comment = TransactionAnomalyMessages.UnexpectedAmount };
            }

            if (TransactionAnomalyRules.IsFrequencySpike(recentTransactions))
            {
                return new AnomalyStatusDto { IsSuspicious = true, Comment = TransactionAnomalyMessages.FrequencySpike };
            }

            if (TransactionAnomalyRules.IsIQRAnomaly(transaction, recentTransactions))
            {
                return new AnomalyStatusDto { IsSuspicious = true, Comment = TransactionAnomalyMessages.IQRAnomaly };
            }

            if (TransactionAnomalyRules.IsZScoreAnomaly(transaction, recentTransactions))
            {
                return new AnomalyStatusDto { IsSuspicious = true, Comment = TransactionAnomalyMessages.ZScoreAnomaly };
            }

            return new AnomalyStatusDto { IsSuspicious = false, Comment = TransactionAnomalyMessages.ExpectedAmount };
        }
    }
}
