using AestusDemoAPI.Domain.Entitites;
using System.Collections.Concurrent;

namespace AestusDemoAPI.Services
{
    public interface ITransactionQueueService
    {
        Task EnqueueAsync(Transaction transaction);
        bool TryDequeue(out Transaction? txtransaction);
    }

    public class TransactionQueueService : ITransactionQueueService
    {
        private readonly ConcurrentQueue<Transaction> _queue = new();
        public Task EnqueueAsync(Transaction transaction)
        {
            _queue.Enqueue(transaction);
            return Task.CompletedTask;
        }

        public bool TryDequeue(out Transaction? transaction)
        {
            return _queue.TryDequeue(out transaction);
        }
    }
}
