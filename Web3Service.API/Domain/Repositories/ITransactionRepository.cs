using Web3Service.API.Domain.Entities;
using Web3Service.API.Domain.ValueObjects;

namespace Web3Service.API.Domain.Repositories;

/// <summary>
/// 交易记录仓储接口
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    /// 根据ID获取交易
    /// </summary>
    Task<TransactionRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据交易哈希获取交易
    /// </summary>
    Task<TransactionRecord?> GetByHashAsync(string transactionHash, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取账户的交易列表
    /// </summary>
    Task<List<TransactionRecord>> GetByAccountIdAsync(
        Guid accountId,
        TransactionStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取交易列表
    /// </summary>
    Task<List<TransactionRecord>> GetTransactionsAsync(
        string? fromAddress = null,
        string? toAddress = null,
        TransactionStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取交易总数
    /// </summary>
    Task<int> GetTransactionCountAsync(
        Guid? accountId = null,
        TransactionStatus? status = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 添加交易
    /// </summary>
    Task<TransactionRecord> AddAsync(TransactionRecord transaction, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 更新交易
    /// </summary>
    Task<TransactionRecord> UpdateAsync(TransactionRecord transaction, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 删除交易
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取待处理的交易
    /// </summary>
    Task<List<TransactionRecord>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取交易统计信息
    /// </summary>
    Task<Dictionary<TransactionStatus, int>> GetTransactionStatisticsAsync(
        Guid? accountId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// 获取交易总金额
    /// </summary>
    Task<decimal> GetTotalTransactionAmountAsync(
        Guid? accountId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);
}
