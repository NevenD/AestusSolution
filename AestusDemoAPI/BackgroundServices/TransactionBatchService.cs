using AestusDemoAPI.Domain.Entitites;
using AestusDemoAPI.Infrastructure;
using AestusDemoAPI.Services;
using Microsoft.EntityFrameworkCore;

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

        private readonly Dictionary<string, List<Transaction>> _userRecentTransactionsCache = new();

        public TransactionBatchService(ITransactionQueueService queue,
                                       IServiceScopeFactory scopeFactory,
                                       IAnomalyDetectionService anomalyDetectionService,
                                       ILogger<TransactionBatchService> logger,
                                       Dictionary<string, List<Transaction>> userRecentTransactionsCache)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _anomalyDetectionService = anomalyDetectionService;
            _logger = logger;
            _userRecentTransactionsCache = userRecentTransactionsCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var batch = new List<Transaction>(BatchSize);

            while (!stoppingToken.IsCancellationRequested)
            {
                while (batch.Count < BatchSize && _queue.TryDequeue(out var transaction))
                {
                    batch.Add(transaction!);
                }

                if (batch.Count > 0)
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<FinTechAestusContext>();
                        foreach (var transaction in batch)
                        {
                            // Load or get cached recent transactions for user
                            if (!_userRecentTransactionsCache.TryGetValue(transaction.UserId, out var recentTransactions))
                            {
                                recentTransactions = await db.Transactions
                                    .Where(t => t.UserId == transaction.UserId)
                                    .OrderByDescending(t => t.Timestamp)
                                    .Take(1000)
                                    .ToListAsync(stoppingToken);

                                _userRecentTransactionsCache[transaction.UserId] = recentTransactions;
                            }

                            // Use cached recent transactions for anomaly detection
                            var anomalyStatus = _anomalyDetectionService.CheckCached(transaction, recentTransactions);
                            transaction.IsSuspicious = anomalyStatus.IsSuspicious;
                            transaction.Comment = anomalyStatus.Comment;

                            UpdateCache(transaction, recentTransactions);

                            // Save all transactions in batch
                            db.Transactions.AddRange(batch);
                            await db.SaveChangesAsync(stoppingToken);

                            _logger.LogInformation("Saved batch of {Count} transactions", batch.Count);
                            batch.Clear();
                        }
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
