using Web3Service.API.Domain.Entities;
using Web3Service.API.Domain.ValueObjects;

namespace Web3Service.API.Domain.Repositories;

/// <summary>
/// 合约部署记录仓储接口
/// </summary>
public interface IContractDeploymentRepository
{
    /// <summary>
    /// 根据ID获取合约部署记录
    /// </summary>
    Task<ContractDeployment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据合约地址获取部署记录
    /// </summary>
    Task<ContractDeployment?> GetByContractAddressAsync(string contractAddress, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据交易哈希获取部署记录
    /// </summary>
    Task<ContractDeployment?> GetByTransactionHashAsync(string transactionHash, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取账户的合约部署列表
    /// </summary>
    Task<List<ContractDeployment>> GetByDeployerAccountIdAsync(
        Guid deployerAccountId,
        ContractType? contractType = null,
        DeploymentStatus? status = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取合约部署列表
    /// </summary>
    Task<List<ContractDeployment>> GetDeploymentsAsync(
        ContractType? contractType = null,
        DeploymentStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取部署记录总数
    /// </summary>
    Task<int> GetDeploymentCountAsync(
        Guid? deployerAccountId = null,
        ContractType? contractType = null,
        DeploymentStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 添加部署记录
    /// </summary>
    Task<ContractDeployment> AddAsync(ContractDeployment deployment, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 更新部署记录
    /// </summary>
    Task<ContractDeployment> UpdateAsync(ContractDeployment deployment, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 删除部署记录
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取待部署的合约
    /// </summary>
    Task<List<ContractDeployment>> GetPendingDeploymentsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取部署统计信息
    /// </summary>
    Task<Dictionary<DeploymentStatus, int>> GetDeploymentStatisticsAsync(
        Guid? deployerAccountId = null,
        ContractType? contractType = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);
}

