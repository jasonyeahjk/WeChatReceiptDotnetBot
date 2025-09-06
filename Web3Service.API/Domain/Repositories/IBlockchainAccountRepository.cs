using Web3Service.API.Domain.Entities;
using Web3Service.API.Domain.ValueObjects;

namespace Web3Service.API.Domain.Repositories;

/// <summary>
/// 区块链账户仓储接口
/// </summary>
public interface IBlockchainAccountRepository
{
    /// <summary>
    /// 根据ID获取账户
    /// </summary>
    Task<BlockchainAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据地址获取账户
    /// </summary>
    Task<BlockchainAccount?> GetByAddressAsync(string address, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取账户列表
    /// </summary>
    Task<List<BlockchainAccount>> GetAccountsAsync(
        AccountType? accountType = null,
        bool? isActive = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取账户总数
    /// </summary>
    Task<int> GetAccountCountAsync(
        AccountType? accountType = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 添加账户
    /// </summary>
    Task<BlockchainAccount> AddAsync(BlockchainAccount account, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 更新账户
    /// </summary>
    Task<BlockchainAccount> UpdateAsync(BlockchainAccount account, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 删除账户
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 检查地址是否存在
    /// </summary>
    Task<bool> ExistsAsync(string address, CancellationToken cancellationToken = default);
}

