
using AestusDemoAPI.Domain.Entitites;
using AestusDemoAPI.Validation;

namespace AestusDemoAPI.Tests
{
    [TestClass]
    public sealed class TransactionAnomalyRulesTests
    {



        [DataTestMethod]
        [DataRow("Zagreb", false)]
        [DataRow("Pregrada", true)]
        [DataRow("zadar", false)]
        [DataRow("  Zagreb  ", false)]
        [DataRow("", true)]
        [DataRow(null, true)]
        public void IsInvalidLocation_ShouldBehaveAsExpected(string location, bool expectedIsInvalid)
        {
            var transaction = new Transaction { Location = location ?? string.Empty, UserId = "u1", Timestamp = DateTime.UtcNow };
            var result = TransactionAnomalyRules.IsInvalidLocation(transaction);
            Assert.AreEqual(expectedIsInvalid, result);
        }

        [DataTestMethod]
        [DataRow(200_000, true)]
        [DataRow(100_001, true)]
        [DataRow(100_000, false)]
        [DataRow(99_999, false)]
        [DataRow(0, false)]
        [DataRow(-1, false)]
        public void IsExtremeAmount_ShouldFlagCorrectly(double amount, bool expectedIsAnomalous)
        {
            var transaction = new Transaction
            {
                Amount = amount,
                UserId = "u1",
                Timestamp = DateTime.UtcNow,
                Location = "TestLocation"
            };

            var result = TransactionAnomalyRules.IsInvalidAmount(transaction);

            Assert.AreEqual(expectedIsAnomalous, result);
        }

        [DataTestMethod]
        [DataRow(15, true)]
        [DataRow(10, false)]
        [DataRow(5, false)]
        public void FrequencySpike_ShouldDetectBasedOnRecentCount(int recentTxCount, bool expectedIsSpike)
        {
            var now = DateTime.UtcNow;

            var transaction = new Transaction
            {
                Timestamp = now,
                UserId = "u1",
                Amount = 100,
                Location = "TestLocation"
            };

            var history = Enumerable.Range(0, recentTxCount)
                .Select(i => new Transaction
                {
                    Timestamp = now.AddSeconds(-i * 10), // simluating transactions with 10 sec increment
                    UserId = "u1",
                    Amount = 100,
                    Location = "TestLocation"
                })
                .ToList();

            var result = TransactionAnomalyRules.IsFrequencySpike(history);

            Assert.AreEqual(expectedIsSpike, result);
        }

        [DataTestMethod]
        [DataRow(100, false)]
        [DataRow(80, true)]
        [DataRow(50, true)]
        public void IQR_ShouldDetectOutliersBasedOnAmount(double testAmount, bool expectedIsAnomaly)
        {
            var now = DateTime.UtcNow;

            var transactions = Enumerable.Range(0, 20)
                .Select(i => new Transaction
                {
                    Amount = 100 + i,
                    UserId = "u1",
                    Timestamp = now,
                    Location = "X"
                })
                .ToList();

            var anomalyTransaction = new Transaction
            {
                Amount = testAmount,
                UserId = "u1",
                Timestamp = now,
                Location = "X"
            };

            var result = TransactionAnomalyRules.IsIQRAnomaly(anomalyTransaction, transactions);

            Assert.AreEqual(expectedIsAnomaly, result);
        }

        [DataTestMethod]
        [DataRow(110, false)]
        [DataRow(95, false)]
        [DataRow(120, false)]
        public void ZScore_ShouldDetectAnomalyBasedOnAmount(double testAmount, bool expectedIsAnomaly)
        {
            var now = DateTime.UtcNow;

            // Arrange: list of normal transactions all with Amount=100
            var transactions = Enumerable.Range(0, 20)
                .Select(i => new Transaction
                {
                    Amount = 100,
                    UserId = "u1",
                    Timestamp = now,
                    Location = "X"
                })
                .ToList();

            var anomalyTransaction = new Transaction
            {
                Amount = testAmount,
                UserId = "u1",
                Timestamp = now,
                Location = "X"
            };

            // Act
            var result = TransactionAnomalyRules.IsZScoreAnomaly(anomalyTransaction, transactions);

            Assert.AreEqual(expectedIsAnomaly, result);
        }
    }
}
