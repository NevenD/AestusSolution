using AestusDemoAPI.Domain.Entitites;

namespace AestusDemoAPI.Validation
{
    public static class TransactionAnomalyRules
    {
        public static class AnomalyRules
        {
            public static bool IsExtremeAmount(Transaction transaction)
            {
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

            public static bool IsZScoreAnomaly(Transaction tx, List<Transaction> txs)
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
                return Math.Abs(z) > 3;
            }
        }
    }
}
