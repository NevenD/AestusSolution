using AestusDemoAPI.Domain.Entitites;
using AestusDemoAPI.Infrastructure;
using AestusDemoAPI.Services;

namespace AestusDemoAPI.BackgroundServices
{
    public class TransactionBatchService : BackgroundService
    {
        private readonly ITransactionQueueService _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TransactionBatchService> _logger;

        private const int BatchSize = 1000;
        private const int BatchDelayMs = 200;

        public TransactionBatchService(ITransactionQueueService queue, IServiceScopeFactory scopeFactory, ILogger<TransactionBatchService> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
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

                        db.Transactions.AddRange(batch);
                        await db.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("Saved batch of {Count} transactions", batch.Count);
                        batch.Clear();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error saving batch of transactions");
                        // Decide how to handle failed batch (retry? move to dead-letter queue?)
                    }
                }
                else
                {
                    // No transactions, wait a bit before next check to avoid tight loop
                    await Task.Delay(BatchDelayMs, stoppingToken);
                }
            }
        }
    }
}
