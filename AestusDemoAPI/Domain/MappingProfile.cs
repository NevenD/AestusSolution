using AestusDemoAPI.Domain.Dtos;
using AestusDemoAPI.Domain.Entitites;
using System.Globalization;

namespace AestusDemoAPI.Domain
{
    public static class MappingProfile
    {
        private static readonly CultureInfo _croatianCulture = new("hr-HR");
        public static TransactionDto ToTransactionDto(this Transaction transaction)
        {
            return new TransactionDto
            {
                UserId = transaction.UserId,
                Amount = transaction.Amount,
                AmountWithCurrency = $"{transaction.Amount.ToString("F2", _croatianCulture)} €",
                Timestamp = transaction.Timestamp.ToString("g", _croatianCulture),
                Location = transaction.Location,
                IsSuspicious = transaction.IsSuspicious,
                Comment = transaction.Comment,
            };
        }

        public static SuspiciousTransactionDto ToSuspiciousTransactionDto(this Transaction transaction)
        {
            return new SuspiciousTransactionDto
            {
                UserId = transaction.UserId,
                Amount = transaction.Amount,
                AmountWithCurrency = $"{transaction.Amount.ToString("F2", _croatianCulture)} €",
                Timestamp = transaction.Timestamp.ToString("g", _croatianCulture),
                Location = transaction.Location,
                Comment = transaction.Comment,
            };
        }

        public static DailySuspiciousSummaryDto ToDailySuspiciousSummaryDto(this Transaction transaction)
        {
            return new DailySuspiciousSummaryDto
            {
                UserId = transaction.UserId,
                AmountWithCurrency = $"{transaction.Amount.ToString("F2", _croatianCulture)} €",
                Timestamp = transaction.Timestamp.ToString("g", _croatianCulture),
                Location = transaction.Location,
                Comment = transaction.Comment,
            };
        }
    }
}
