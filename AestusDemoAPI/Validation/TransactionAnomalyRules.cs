using AestusDemoAPI.Domain.Entitites;

namespace AestusDemoAPI.Validation
{
    public static class TransactionAnomalyRules
    {
        public static readonly List<string> Locations =
        [
            "Zagreb",
            "Split",
            "Rijeka",
            "Osijek",
            "Zadar"
        ];

        public static bool IsInvalidLocation(Transaction transaction)
        {
            return !Locations.Any(x => string.Equals(x.Trim(), transaction.Location.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsInvalidAmount(Transaction transaction)
        {
            if (transaction.Amount < 0)
            {
                return false;
            }

            return transaction.Amount > 100_000;
        }

        public static bool IsFrequencySpike(List<Transaction> recentTransactions)
        {
            var fiveMinutesAgo = DateTime.UtcNow.AddMinutes(-5);
            var count = recentTransactions.Count(t => t.Timestamp > fiveMinutesAgo);
            return count > 10;
        }

        public static bool IsIQRAnomaly(Transaction tx, List<Transaction> txs)
        {
            if (txs.Count < 10)
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

        public static bool IsZScoreAnomaly(Transaction transaction, List<Transaction> transactions)
        {
            var standardDeviation = 2;

            if (transactions.Count < 10)
            {
                return false;
            }

            var amounts = transactions.Select(t => t.Amount).ToList();
            var mean = amounts.Average();

            // Sample variance (divide by n-1)
            var variance = amounts.Sum(a => Math.Pow((double)(a - mean), 2)) / (amounts.Count - 1);
            var stdDev = Math.Sqrt(variance);

            if (stdDev == 0)
            {
                return false;
            }

            var z = (transaction.Amount - mean) / stdDev;
            return Math.Abs(z) > standardDeviation;
        }
    }
}
