using FluentResults;
using Web3Service.API.Domain.Entities;

namespace Web3Service.API.Domain.Services;

/// <summary>
/// Web3区块链服务接口
/// </summary>
public interface IWeb3BlockchainService
{
    /// <summary>
    /// 创建新账户
    /// </summary>
    Task<Result<BlockchainAccount>> CreateAccountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 从私钥导入账户
    /// </summary>
    Task<Result<BlockchainAccount>> ImportAccountFromPrivateKeyAsync(string privateKey, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取账户余额
    /// </summary>
    Task<Result<decimal>> GetBalanceAsync(string address, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 发送交易
    /// </summary>
    Task<Result<string>> SendTransactionAsync(
        string fromAddress,
        string toAddress,
        decimal amount,
        string? privateKey = null,
        decimal? gasPrice = null,
        long? gasLimit = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取交易详情
    /// </summary>
    Task<Result<TransactionRecord>> GetTransactionAsync(string transactionHash, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取交易收据
    /// </summary>
    Task<Result<object>> GetTransactionReceiptAsync(string transactionHash, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 估算Gas费用
    /// </summary>
    Task<Result<long>> EstimateGasAsync(
        string fromAddress,
        string toAddress,
        decimal amount,
        string? data = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取当前Gas价格
    /// </summary>
    Task<Result<decimal>> GetGasPriceAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取最新区块号
    /// </summary>
    Task<Result<long>> GetLatestBlockNumberAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取区块信息
    /// </summary>
    Task<Result<object>> GetBlockAsync(long blockNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取网络ID
    /// </summary>
    Task<Result<int>> GetNetworkIdAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 检查网络连接状态
    /// </summary>
    Task<Result<bool>> IsConnectedAsync(CancellationToken cancellationToken = default);
}

