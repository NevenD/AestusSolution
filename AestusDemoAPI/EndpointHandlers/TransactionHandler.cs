using AestusDemoAPI.Domain;
using AestusDemoAPI.Domain.Entitites;
using AestusDemoAPI.Infrastructure;
using AestusDemoAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace AestusDemoAPI.EndpointHandlers
{
    public class TransactionHandler
    {
        public static async Task<IResult> PostTransactionAsync(Transaction transaction, ITransactionQueueService transactionQueueService)
        {
            await transactionQueueService.EnqueueAsync(transaction);
            return Results.Accepted();
        }

        public static async Task<IResult> GetAnomaliesAsync(string id, FinTechAestusContext db)
        {
            var anomalies = await db.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == id && t.IsSuspicious)
                .Select(t => t.ToSuspiciousTransactionDto())
                .ToListAsync();

            return Results.Ok(anomalies);
        }

        public static async Task<IResult> GetTransactionsAsync(FinTechAestusContext db)
        {
            var anomalies = await db.Transactions
                .AsNoTracking()
                .Select(t => t.ToTransactionDto())
                .ToListAsync();

            return Results.Ok(anomalies);
        }
    }
}
