using AestusDemoAPI.Domain.Entitites;
using AestusDemoAPI.Services;
using AestusDemoAPI.Settings;
using AestusDemoAPI.Validation;
using Microsoft.Extensions.Options;

namespace AestusDemoAPI.Tests
{
    [TestClass]
    public class AnomalyTransactionServiceTests
    {
        private AnomalyTransactionService? _service;

        [TestInitialize]
        public void Setup()
        {
            var validationSettings = new ValidationSettings
            {
                Locations = ["Zagreb", "Split", "Rijeka"],
                MaxAmount = 100000,
                AnomalyCount = 10,
                BatchDelayMs = 200
            };

            var options = Options.Create(validationSettings);

            _service = new AnomalyTransactionService(options);
        }


        [DataTestMethod]
        [DataRow("Pregrada", TransactionAnomalyMessages.UnexpectedLocation)]
        [DataRow("", TransactionAnomalyMessages.UnexpectedLocation)]
        [DataRow(null, TransactionAnomalyMessages.UnexpectedLocation)]
        public void CheckCached_ShouldReturnInvalidLocation(string location, string message)
        {
            var transaction = new Transaction { Location = location ?? string.Empty, UserId = "u1", Timestamp = DateTime.UtcNow };
            var recentTransactions = new List<Transaction>();

            var result = _service!.CheckCached(transaction, recentTransactions);

            Assert.IsTrue(result.IsSuspicious);
            Assert.AreEqual(message, result.Comment);
        }

        [DataTestMethod]
        [DataRow(200_000)]
        [DataRow(100_001)]
        public void CheckCached_ShouldReturnUnexpectedAmount(double amount)
        {
            var transaction = new Transaction { Location = "Zagreb" ?? string.Empty, UserId = "u1", Timestamp = DateTime.UtcNow, Amount = amount };
            var recentTransactions = new List<Transaction>();

            var result = _service!.CheckCached(transaction, recentTransactions);

            Assert.IsTrue(result.IsSuspicious);
            Assert.AreEqual(TransactionAnomalyMessages.UnexpectedAmount, result.Comment);
        }

        [TestMethod]
        public void CheckCached_ShouldReturnFrequencySpike()
        {
            var transaction = new Transaction
            {
                Location = "Zagreb",
                Amount = 100,
                UserId = "u1",
                Timestamp = DateTime.UtcNow
            };

            var recentTransactions = Enumerable.Range(1, 11)
                .Select(i => new Transaction
                {
                    UserId = transaction.UserId,
                    Amount = 100,
                    Location = "Zagreb",
                    Timestamp = DateTime.UtcNow.AddMinutes(-i / 10.0)
                })
                .ToList();

            var result = _service!.CheckCached(transaction, recentTransactions);

            Assert.IsTrue(result.IsSuspicious);
            Assert.AreEqual(TransactionAnomalyMessages.FrequencySpike, result.Comment);
        }

        [TestMethod]
        public void CheckCached_ShouldReturnIQRAnomaly()
        {
            var normalAmounts = Enumerable.Range(1, 20)
                .Select(i => new Transaction
                {
                    UserId = "test1",
                    Location = "Zagreb",
                    Amount = i * 10
                })
                .ToList();

            var anomalyTransaction = new Transaction
            {
                UserId = "test1",
                Amount = 1000,
                Location = "Zagreb",
                Timestamp = DateTime.UtcNow
            };

            var result = _service!.CheckCached(anomalyTransaction, normalAmounts);

            Assert.IsTrue(result.IsSuspicious);
            Assert.AreEqual(TransactionAnomalyMessages.IQRAnomaly, result.Comment);
        }


        [DataTestMethod]
        [DataRow(110)]
        [DataRow(95)]
        [DataRow(120)]
        public void CheckCached_ShouldReturnExpectedAmount(double testAmount)
        {
            var now = DateTime.UtcNow;
            var transactions = Enumerable.Range(0, 5)
                .Select(i => new Transaction
                {
                    Amount = 100,
                    UserId = "u1",
                    Location = "Zagreb",
                    Timestamp = now,
                })
                .ToList();

            var anomalyTransaction = new Transaction
            {
                Amount = testAmount,
                UserId = "u1",
                Timestamp = now,
                Location = "Zagreb",

            };

            var result = _service!.CheckCached(anomalyTransaction, transactions);

            Assert.IsFalse(result.IsSuspicious);
            Assert.AreEqual(TransactionAnomalyMessages.ExpectedAmount, result.Comment);
        }

    }
}
