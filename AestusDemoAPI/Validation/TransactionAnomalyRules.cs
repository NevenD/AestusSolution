using AestusDemoAPI.Domain.Entitites;

namespace AestusDemoAPI.Validation
{
    public static class TransactionAnomalyRules
    {

        public static bool IsInvalidLocation(Transaction transaction, List<string> locations)
        {
            return !locations.Any(x => string.Equals(x.Trim(), transaction.Location.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsUnexpectedAmount(Transaction transaction, double amount)
        {
            if (transaction.Amount < 0)
            {
                return false;
            }

            return transaction.Amount > amount;
        }

        public static bool IsFrequencySpike(List<Transaction> recentTransactions, int anomalyCount)
        {
            var fiveMinutesAgo = DateTime.UtcNow.AddMinutes(-5);
            var count = recentTransactions.Count(t => t.Timestamp > fiveMinutesAgo);
            return count > anomalyCount;
        }

        public static bool IsIQRAnomaly(Transaction tx, List<Transaction> txs, int anomalyCount)
        {
            if (txs.Count < anomalyCount)
            {
                return false;
            }

            var sorted = txs.Select(t => t.Amount).OrderBy(a => a).ToList();
            var q1 = sorted[sorted.Count / 4];
            var q3 = sorted[3 * sorted.Count / 4];
            var iqr = q3 - q1;

            var lower = q1 - (1.5 * iqr);
            var upper = q3 + (1.5 * iqr);

            return tx.Amount < lower || tx.Amount > upper;
        }
    }
}
