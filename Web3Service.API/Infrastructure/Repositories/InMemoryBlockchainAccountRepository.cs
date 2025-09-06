using Web3Service.API.Domain.Entities;
using Web3Service.API.Domain.Repositories;
using Web3Service.API.Domain.ValueObjects;

namespace Web3Service.API.Infrastructure.Repositories;

/// <summary>
/// 内存区块链账户仓储实现
/// </summary>
public class InMemoryBlockchainAccountRepository : IBlockchainAccountRepository
{
    private readonly List<BlockchainAccount> _accounts = new();
    private readonly object _lock = new();

    public Task<BlockchainAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var account = _accounts.FirstOrDefault(a => a.Id == id);
            return Task.FromResult(account);
        }
    }

    public Task<BlockchainAccount?> GetByAddressAsync(string address, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var account = _accounts.FirstOrDefault(a => a.Address.Equals(address, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(account);
        }
    }

    public Task<List<BlockchainAccount>> GetAccountsAsync(
        AccountType? accountType = null,
        bool? isActive = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var query = _accounts.AsQueryable();

            if (accountType.HasValue)
            {
                query = query.Where(a => a.AccountType == accountType.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(a => a.IsActive == isActive.Value);
            }

            var result = query
                .OrderByDescending(a => a.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToList();

            return Task.FromResult(result);
        }
    }

    public Task<int> GetAccountCountAsync(
        AccountType? accountType = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var query = _accounts.AsQueryable();

            if (accountType.HasValue)
            {
                query = query.Where(a => a.AccountType == accountType.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(a => a.IsActive == isActive.Value);
            }

            return Task.FromResult(query.Count());
        }
    }

    public Task<BlockchainAccount> AddAsync(BlockchainAccount account, CancellationToken cancellationToken = default)
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account));

        lock (_lock)
        {
            _accounts.Add(account);
            return Task.FromResult(account);
        }
    }

    public Task<BlockchainAccount> UpdateAsync(BlockchainAccount account, CancellationToken cancellationToken = default)
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account));

        lock (_lock)
        {
            var existingIndex = _accounts.FindIndex(a => a.Id == account.Id);
            if (existingIndex >= 0)
            {
                _accounts[existingIndex] = account;
            }
            return Task.FromResult(account);
        }
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var account = _accounts.FirstOrDefault(a => a.Id == id);
            if (account != null)
            {
                _accounts.Remove(account);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<bool> ExistsAsync(string address, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var exists = _accounts.Any(a => a.Address.Equals(address, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(exists);
        }
    }
}

