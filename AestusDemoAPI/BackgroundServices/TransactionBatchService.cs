using AestusDemoAPI.Domain.Entitites;
using AestusDemoAPI.Infrastructure;
using AestusDemoAPI.Services;
using AestusDemoAPI.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace AestusDemoAPI.BackgroundServices
{
    public sealed class TransactionBatchService : BackgroundService
    {
        private readonly ITransactionQueueService _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IAnomalyDetectionService _anomalyDetectionService;
        private readonly ILogger<TransactionBatchService> _logger;

        private readonly TransactionSettings _settings;

        private readonly ConcurrentDictionary<string, List<Transaction>> _userRecentTransactionsCache = new();

        public TransactionBatchService(IOptions<TransactionSettings> options,
                                       ITransactionQueueService queue,
                                       IServiceScopeFactory scopeFactory,
                                       IAnomalyDetectionService anomalyDetectionService,
                                       ILogger<TransactionBatchService> logger)
        {
            _queue = queue;
            _settings = options.Value;
            _scopeFactory = scopeFactory;
            _anomalyDetectionService = anomalyDetectionService;
            _logger = logger;
        }



        /// <summary>
        /// Executes the background service loop that batches incoming transactions from the queue.
        /// When a batch is ready (by size or timeout), it processes each transaction for anomaly detection,
        /// updates the user transaction cache, and persists the batch to the database.
        /// Handles errors and logs batch operations.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var transactionBatch = new List<Transaction>(_settings.BatchSize);
            DateTime? batchStartTime = null;
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var transaction))
                {
                    transactionBatch.Add(transaction!);
                    if (batchStartTime is null)
                    {
                        batchStartTime = DateTime.UtcNow;
                    }
                }

                bool batchReady = IsBatchReady(transactionBatch, batchStartTime, _settings.BatchTimeoutSeconds, _settings.BatchSize);

                if (batchReady)
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<FinTechAestusContext>();
                        foreach (var trans in transactionBatch)
                        {
                            if (!_userRecentTransactionsCache.TryGetValue(trans.UserId, out var recentTransactions))
                            {
                                recentTransactions = await db.Transactions
                                    .Where(t => t.UserId == trans.UserId)
                                    .OrderByDescending(t => t.Timestamp)
                                    .Take(1000)
                                    .ToListAsync(stoppingToken);

                                _userRecentTransactionsCache[trans.UserId] = recentTransactions;
                            }

                            var anomalyStatus = _anomalyDetectionService.CheckCached(trans, recentTransactions);
                            trans.IsSuspicious = anomalyStatus.IsSuspicious;
                            trans.Comment = anomalyStatus.Comment;

                            UpdateCache(trans, recentTransactions, _settings.BatchSize);
                        }

                        db.Transactions.AddRange(transactionBatch);
                        await db.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("Saved batch of {Count} transactions", transactionBatch.Count);
                        transactionBatch.Clear();
                        batchStartTime = null;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error saving batch of transactions");
                    }
                }
                else
                {
                    await Task.Delay(_settings.BatchDelayMs, stoppingToken);
                }
            }
        }


        /// <summary>
        /// Determines whether the current batch of transactions is ready to be processed.
        /// A batch is considered ready if it has reached the specified size, or if the timeout
        /// since the first transaction was added has elapsed.
        /// </summary>
        /// <param name="transactionBatch">The list of transactions currently in the batch.</param>
        /// <param name="batchStartTime">The time when the first transaction was added to the batch, or null if the batch is empty.</param>
        /// <param name="batchTimeoutSeconds">The maximum number of seconds to wait before processing a non-full batch.</param>
        /// <param name="batchSize">The maximum number of transactions per batch.</param>
        /// <returns>True if the batch is ready to be processed; otherwise, false.</returns>
        private static bool IsBatchReady(List<Transaction> transactionBatch, DateTime? batchStartTime, int batchTimeoutSeconds, int batchSize)
        {
            bool timeoutReached = batchStartTime.HasValue &&
                                  (DateTime.UtcNow - batchStartTime.Value).TotalSeconds >= batchTimeoutSeconds;

            bool batchReady = transactionBatch.Count >= batchSize || (transactionBatch.Count > 0 && timeoutReached);
            return batchReady;
        }


        /// <summary>
        /// Updates the recent transactions cache for a user by inserting the latest transaction at the beginning of the list.
        /// Ensures the cache does not exceed the specified batch size by removing the oldest transaction if necessary.
        /// </summary>
        /// <param name="transaction">The new transaction to add to the cache.</param>
        /// <param name="recentTransactions">The list of recent transactions for the user.</param>
        /// <param name="batchSize">The maximum number of transactions to keep in the cache.</param>
        private static void UpdateCache(Transaction transaction, List<Transaction> recentTransactions, int batchSize)
        {
            recentTransactions.Insert(0, transaction);
            if (recentTransactions.Count > batchSize)
            {
                recentTransactions.RemoveAt(recentTransactions.Count - 1);
            }
        }
    }
}
