using AestusDemoAPI.Domain.Dtos;
using AestusDemoAPI.Domain.Entitites;
using AestusDemoAPI.Settings;
using AestusDemoAPI.Validation;
using Microsoft.Extensions.Options;

namespace AestusDemoAPI.Services
{
    public interface IAnomalyDetectionService
    {

        AnomalyStatusDto CheckCached(Transaction transaction, List<Transaction> recentTransactions);
    }

    public class AnomalyTransactionService : IAnomalyDetectionService
    {
        private readonly ValidationSettings _settings;

        public AnomalyTransactionService(IOptions<ValidationSettings> options)
        {
            _settings = options.Value;
        }

        public AnomalyStatusDto CheckCached(Transaction transaction, List<Transaction> recentTransactions)
        {

            if (TransactionAnomalyRules.IsInvalidLocation(transaction, _settings.Locations))
            {
                return new AnomalyStatusDto { IsSuspicious = true, Comment = TransactionAnomalyMessages.UnexpectedLocation };
            }

            if (TransactionAnomalyRules.IsUnexpectedAmount(transaction, _settings.MaxAmount))
            {
                return new AnomalyStatusDto { IsSuspicious = true, Comment = TransactionAnomalyMessages.UnexpectedAmount };
            }

            if (TransactionAnomalyRules.IsFrequencySpike(recentTransactions, _settings.AnomalyCount))
            {
                return new AnomalyStatusDto { IsSuspicious = true, Comment = TransactionAnomalyMessages.FrequencySpike };
            }

            if (TransactionAnomalyRules.IsIQRAnomaly(transaction, recentTransactions, _settings.AnomalyCount))
            {
                return new AnomalyStatusDto { IsSuspicious = true, Comment = TransactionAnomalyMessages.IQRAnomaly };
            }

            return new AnomalyStatusDto { IsSuspicious = false, Comment = TransactionAnomalyMessages.ExpectedAmount };
        }
    }
}
