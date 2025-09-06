using Web3Service.API.Domain.Entities;
using Web3Service.API.Domain.Repositories;
using Web3Service.API.Domain.ValueObjects;

namespace Web3Service.API.Infrastructure.Repositories;

/// <summary>
/// 内存合约部署仓储实现
/// </summary>
public class InMemoryContractDeploymentRepository : IContractDeploymentRepository
{
    private readonly List<ContractDeployment> _deployments = new();
    private readonly object _lock = new();

    public Task<ContractDeployment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var deployment = _deployments.FirstOrDefault(d => d.Id == id);
            return Task.FromResult(deployment);
        }
    }

    public Task<ContractDeployment?> GetByContractAddressAsync(string contractAddress, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var deployment = _deployments.FirstOrDefault(d => d.ContractAddress.Equals(contractAddress, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(deployment);
        }
    }

    public Task<ContractDeployment?> GetByTransactionHashAsync(string transactionHash, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var deployment = _deployments.FirstOrDefault(d => d.DeploymentTransactionHash.Equals(transactionHash, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(deployment);
        }
    }

    public Task<List<ContractDeployment>> GetByDeployerAccountIdAsync(
        Guid deployerAccountId,
        ContractType? contractType = null,
        DeploymentStatus? status = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var query = _deployments.Where(d => d.DeployerAccountId == deployerAccountId).AsQueryable();

            if (contractType.HasValue)
            {
                query = query.Where(d => d.ContractType == contractType.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(d => d.Status == status.Value);
            }

            var result = query
                .OrderByDescending(d => d.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToList();

            return Task.FromResult(result);
        }
    }

    public Task<List<ContractDeployment>> GetDeploymentsAsync(
        ContractType? contractType = null,
        DeploymentStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var query = _deployments.AsQueryable();

            if (contractType.HasValue)
            {
                query = query.Where(d => d.ContractType == contractType.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(d => d.Status == status.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(d => d.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(d => d.CreatedAt <= toDate.Value);
            }

            var result = query
                .OrderByDescending(d => d.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToList();

            return Task.FromResult(result);
        }
    }

    public Task<int> GetDeploymentCountAsync(
        Guid? deployerAccountId = null,
        ContractType? contractType = null,
        DeploymentStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var query = _deployments.AsQueryable();

            if (deployerAccountId.HasValue)
            {
                query = query.Where(d => d.DeployerAccountId == deployerAccountId.Value);
            }

            if (contractType.HasValue)
            {
                query = query.Where(d => d.ContractType == contractType.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(d => d.Status == status.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(d => d.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(d => d.CreatedAt <= toDate.Value);
            }

            return Task.FromResult(query.Count());
        }
    }

    public Task<ContractDeployment> AddAsync(ContractDeployment deployment, CancellationToken cancellationToken = default)
    {
        if (deployment == null)
            throw new ArgumentNullException(nameof(deployment));

        lock (_lock)
        {
            _deployments.Add(deployment);
            return Task.FromResult(deployment);
        }
    }

    public Task<ContractDeployment> UpdateAsync(ContractDeployment deployment, CancellationToken cancellationToken = default)
    {
        if (deployment == null)
            throw new ArgumentNullException(nameof(deployment));

        lock (_lock)
        {
            var existingIndex = _deployments.FindIndex(d => d.Id == deployment.Id);
            if (existingIndex >= 0)
            {
                _deployments[existingIndex] = deployment;
            }
            return Task.FromResult(deployment);
        }
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var deployment = _deployments.FirstOrDefault(d => d.Id == id);
            if (deployment != null)
            {
                _deployments.Remove(deployment);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }

    public Task<List<ContractDeployment>> GetPendingDeploymentsAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var pendingDeployments = _deployments
                .Where(d => d.Status == DeploymentStatus.Pending)
                .OrderBy(d => d.CreatedAt)
                .ToList();

            return Task.FromResult(pendingDeployments);
        }
    }

    public Task<Dictionary<DeploymentStatus, int>> GetDeploymentStatisticsAsync(
        Guid? deployerAccountId = null,
        ContractType? contractType = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var query = _deployments.AsQueryable();

            if (deployerAccountId.HasValue)
            {
                query = query.Where(d => d.DeployerAccountId == deployerAccountId.Value);
            }

            if (contractType.HasValue)
            {
                query = query.Where(d => d.ContractType == contractType.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(d => d.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(d => d.CreatedAt <= toDate.Value);
            }

            var statistics = query
                .GroupBy(d => d.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            return Task.FromResult(statistics);
        }
    }
}

