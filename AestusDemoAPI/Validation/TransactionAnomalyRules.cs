using AestusDemoAPI.Domain.Entitites;

namespace AestusDemoAPI.Validation
{
    public static class TransactionAnomalyRules
    {

        /// <summary>
        /// Determines whether the transaction location is invalid based on a list of allowed locations.
        /// </summary>
        /// <param name="transaction">The transaction to validate.</param>
        /// <param name="locations">A list of valid location names.</param>
        /// <returns>
        /// True if the transaction's location does not match any of the allowed locations (case-insensitive); otherwise, false.
        /// </returns>
        public static bool IsInvalidLocation(Transaction transaction, List<string> locations)
        {
            return !locations.Any(x => string.Equals(x.Trim(), transaction.Location.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if the transaction's amount is unexpectedly high compared to a specified threshold.
        /// </summary>
        /// <param name="transaction">The transaction to evaluate.</param>
        /// <param name="amount">The threshold amount to compare against.</param>
        /// <returns>
        /// True if the transaction amount is non-negative and greater than the specified threshold; otherwise, false.
        /// </returns>
        public static bool IsUnexpectedAmount(Transaction transaction, double amount)
        {
            if (transaction.Amount < 0)
            {
                return false;
            }

            return transaction.Amount > amount;
        }

        /// <summary>
        /// Determines if there is a spike in transaction frequency within the last five minutes.
        /// </summary>
        /// <param name="recentTransactions">A list of recent transactions to analyze.</param>
        /// <param name="anomalyCount">The threshold count for what is considered a frequency spike.</param>
        /// <returns>
        /// True if the number of transactions in the last five minutes exceeds the specified anomaly count; otherwise, false.
        /// </returns>
        public static bool IsFrequencySpike(List<Transaction> recentTransactions, int anomalyCount)
        {
            var fiveMinutesAgo = DateTime.UtcNow.AddMinutes(-5);
            var count = recentTransactions.Count(t => t.Timestamp > fiveMinutesAgo);
            return count > anomalyCount;
        }

        /// <summary>
        /// Determines whether a transaction is an anomaly based on the Interquartile Range (IQR) method.
        /// </summary>
        /// <param name="transaction">The transaction to evaluate for anomaly.</param>
        /// <param name="transactions">A list of transactions to use for IQR calculation.</param>
        /// <param name="anomalyCount">The minimum number of transactions required to perform the IQR check.</param>
        /// <returns>
        /// True if the transaction amount is outside the calculated IQR bounds (i.e., less than Q1 - 1.5 * IQR or greater than Q3 + 1.5 * IQR); otherwise, false.
        /// Returns false if the number of transactions is less than the specified anomaly count.
        /// </returns>
        public static bool IsIQRAnomaly(Transaction transaction, List<Transaction> transactions, int anomalyCount)
        {
            if (transactions.Count < anomalyCount)
            {
                return false;
            }

            var sorted = transactions.Select(t => t.Amount).OrderBy(a => a).ToList();
            var q1 = sorted[sorted.Count / 4];
            var q3 = sorted[3 * sorted.Count / 4];
            var iqr = q3 - q1;

            var lower = q1 - (1.5 * iqr);
            var upper = q3 + (1.5 * iqr);

            return transaction.Amount < lower || transaction.Amount > upper;
        }
    }
}
