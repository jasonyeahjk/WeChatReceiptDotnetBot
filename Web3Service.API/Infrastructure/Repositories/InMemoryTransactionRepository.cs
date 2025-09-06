using Web3Service.API.Domain.Entities;
using Web3Service.API.Domain.Repositories;
using Web3Service.API.Domain.ValueObjects;

namespace Web3Service.API.Infrastructure.Repositories;

/// <summary>
/// 内存交易仓储实现
/// </summary>
public class InMemoryTransactionRepository : ITransactionRepository
{
    private readonly List<TransactionRecord> _transactions = new();
    private readonly object _lock = new();

    public Task<TransactionRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var transaction = _transactions.FirstOrDefault(t => t.Id == id);
            return Task.FromResult(transaction);
        }
    }

    public Task<TransactionRecord?> GetByHashAsync(string transactionHash, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var transaction = _transactions.FirstOrDefault(t => t.TransactionHash.Equals(transactionHash, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(transaction);
        }
    }

    public Task<List<TransactionRecord>> GetByAccountIdAsync(
        Guid accountId,
        TransactionStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var query = _transactions.Where(t => t.AccountId == accountId).AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= toDate.Value);
            }

            var result = query
                .OrderByDescending(t => t.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToList();

            return Task.FromResult(result);
        }
    }

    public Task<List<TransactionRecord>> GetTransactionsAsync(
        string? fromAddress = null,
        string? toAddress = null,
        TransactionStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var query = _transactions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(fromAddress))
            {
                query = query.Where(t => t.FromAddress.Equals(fromAddress, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(toAddress))
            {
                query = query.Where(t => t.ToAddress.Equals(toAddress, StringComparison.OrdinalIgnoreCase));
            }

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= toDate.Value);
            }

            var result = query
                .OrderByDescending(t => t.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToList();

            return Task.FromResult(result);
        }
    }

    public Task<int> GetTransactionCountAsync(
        Guid? accountId = null,
        TransactionStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var query = _transactions.AsQueryable();

            if (accountId.HasValue)
            {
                query = query.Where(t => t.AccountId == accountId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= toDate.Value);
            }

            return Task.FromResult(query.Count());
        }
    }

    public Task<TransactionRecord> AddAsync(TransactionRecord transaction, CancellationToken cancellationToken = default)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        lock (_lock)
        {
            _transactions.Add(transaction);
            return Task.FromResult(transaction);
        }
    }

    public Task<TransactionRecord> UpdateAsync(TransactionRecord transaction, CancellationToken cancellationToken = default)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        lock (_lock)
        {
            var existingIndex = _transactions.FindIndex(t => t.Id == transaction.Id);
            if (existingIndex >= 0)
            {
                _transactions[existingIndex] = transaction;
            }
            return Task.FromResult(transaction);
        }
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var transaction = _transactions.FirstOrDefault(t => t.Id == id);
            if (transaction != null)
            {
                _transactions.Remove(transaction);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<List<TransactionRecord>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var pendingTransactions = _transactions
                .Where(t => t.Status == TransactionStatus.Pending)
                .OrderBy(t => t.CreatedAt)
                .ToList();

            return Task.FromResult(pendingTransactions);
        }
    }

    public Task<Dictionary<TransactionStatus, int>> GetTransactionStatisticsAsync(
        Guid? accountId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var query = _transactions.AsQueryable();

            if (accountId.HasValue)
            {
                query = query.Where(t => t.AccountId == accountId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= toDate.Value);
            }

            var statistics = query
                .GroupBy(t => t.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            return Task.FromResult(statistics);
        }
    }
    public Task<decimal> GetTotalTransactionAmountAsync(
        Guid? accountId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var query = _transactions.AsQueryable();

            if (accountId.HasValue)
            {
                query = query.Where(t => t.AccountId == accountId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= toDate.Value);
            }

            return Task.FromResult(query.Sum(t => t.Amount));
        }
    }
}
