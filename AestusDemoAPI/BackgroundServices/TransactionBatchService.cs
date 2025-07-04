using AestusDemoAPI.Domain.Entitites;
using AestusDemoAPI.Infrastructure;
using AestusDemoAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace AestusDemoAPI.BackgroundServices
{
    public class TransactionBatchService : BackgroundService
    {
        private readonly ITransactionQueueService _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IAnomalyDetectionService _anomalyDetectionService;
        private readonly ILogger<TransactionBatchService> _logger;

        private const int BatchSize = 1000;
        private const int BatchDelayMs = 200;

        private readonly ConcurrentDictionary<string, List<Transaction>> _userRecentTransactionsCache = new();

        public TransactionBatchService(ITransactionQueueService queue,
                                       IServiceScopeFactory scopeFactory,
                                       IAnomalyDetectionService anomalyDetectionService,
                                       ILogger<TransactionBatchService> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _anomalyDetectionService = anomalyDetectionService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var transactionBatch = new List<Transaction>(BatchSize);
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

                bool batchReady = IsBatchReady(transactionBatch, batchStartTime);

                if (batchReady)
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<FinTechAestusContext>();
                        foreach (var trans in transactionBatch)
                        {
                            // Load or get cached recent transactions for user
                            if (!_userRecentTransactionsCache.TryGetValue(trans.UserId, out var recentTransactions))
                            {
                                recentTransactions = await db.Transactions
                                    .Where(t => t.UserId == trans.UserId)
                                    .OrderByDescending(t => t.Timestamp)
                                    .Take(1000)
                                    .ToListAsync(stoppingToken);

                                _userRecentTransactionsCache[trans.UserId] = recentTransactions;
                            }

                            var safeRecent = recentTransactions.ToList();

                            var anomalyStatus = _anomalyDetectionService.CheckCached(trans, safeRecent);
                            trans.IsSuspicious = anomalyStatus.IsSuspicious;
                            trans.Comment = anomalyStatus.Comment;

                            UpdateCache(trans, recentTransactions);
                        }

                        // Save all transactions in batch
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
                    // No transactions, wait a bit before next check to avoid tight loop
                    await Task.Delay(BatchDelayMs, stoppingToken);
                }
            }
        }

        private static bool IsBatchReady(List<Transaction> transactionBatch, DateTime? batchStartTime)
        {
            bool timeoutReached = batchStartTime.HasValue &&
                                  (DateTime.UtcNow - batchStartTime.Value).TotalSeconds >= 10;

            bool batchReady = transactionBatch.Count >= BatchSize || (transactionBatch.Count > 0 && timeoutReached);
            return batchReady;
        }

        private static void UpdateCache(Transaction transaction, List<Transaction> recentTransactions)
        {
            recentTransactions.Insert(0, transaction);
            if (recentTransactions.Count > 1000)
            {
                recentTransactions.RemoveAt(recentTransactions.Count - 1);
            }
        }
    }
}
