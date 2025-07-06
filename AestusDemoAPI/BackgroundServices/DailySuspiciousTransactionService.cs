
using AestusDemoAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AestusDemoAPI.BackgroundServices
{
    public sealed class DailySuspiciousTransactionService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DailySuspiciousTransactionService> _logger;

        public DailySuspiciousTransactionService(IServiceScopeFactory scopeFactory,
                                                 ILogger<DailySuspiciousTransactionService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var nextRunTime = now.Date.AddDays(1);
                var delay = nextRunTime - now;

                await Task.Delay(delay, stoppingToken);

                try
                {
                    await ScanAndLogSuspiciousTransactions(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error scanning suspicious transactions");
                }
            }
        }


        ///<summary>
        /// Scans the database for suspicious transactions that occurred in the last 24 hours
        /// and logs the total count found.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        private async Task ScanAndLogSuspiciousTransactions(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FinTechAestusContext>();

            var from = DateTime.UtcNow.AddDays(-1);
            var to = DateTime.UtcNow;
            int suspiciousCount = await db.Transactions
                .AsNoTracking()
                .CountAsync(t => t.IsSuspicious && t.Timestamp >= from.Date && t.Timestamp <= to.Date, cancellationToken);

            _logger.LogInformation("Suspicious transactions in last 24 hours : {Count}", suspiciousCount);
        }
    }
}
