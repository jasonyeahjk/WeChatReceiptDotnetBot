using Web3Service.API.Domain.ValueObjects;

namespace Web3Service.API.Domain.Entities;

/// <summary>
/// 区块链交易记录实体
/// </summary>
public class TransactionRecord
{
    public Guid Id { get; private set; }
    public string TransactionHash { get; private set; }
    public string FromAddress { get; private set; }
    public string ToAddress { get; private set; }
    public decimal Amount { get; private set; }
    public decimal GasPrice { get; private set; }
    public long GasLimit { get; private set; }
    public long? GasUsed { get; private set; }
    public TransactionStatus Status { get; private set; }
    public long? BlockNumber { get; private set; }
    public string? BlockHash { get; private set; }
    public int? TransactionIndex { get; private set; }
    public string? ContractAddress { get; private set; }
    public string? InputData { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    // 外键
    public Guid AccountId { get; private set; }
    public BlockchainAccount Account { get; private set; } = null!;

    private TransactionRecord() { } // EF Core

    public TransactionRecord(
        string transactionHash,
        string fromAddress,
        string toAddress,
        decimal amount,
        decimal gasPrice,
        long gasLimit,
        Guid accountId,
        string? inputData = null)
    {
        Id = Guid.NewGuid();
        TransactionHash = transactionHash ?? throw new ArgumentNullException(nameof(transactionHash));
        FromAddress = fromAddress ?? throw new ArgumentNullException(nameof(fromAddress));
        ToAddress = toAddress ?? throw new ArgumentNullException(nameof(toAddress));
        Amount = amount;
        GasPrice = gasPrice;
        GasLimit = gasLimit;
        AccountId = accountId;
        InputData = inputData;
        Status = TransactionStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新交易状态为已确认
    /// </summary>
    public void MarkAsConfirmed(
        long blockNumber,
        string blockHash,
        int transactionIndex,
        long gasUsed,
        string? contractAddress = null)
    {
        Status = TransactionStatus.Confirmed;
        BlockNumber = blockNumber;
        BlockHash = blockHash;
        TransactionIndex = transactionIndex;
        GasUsed = gasUsed;
        ContractAddress = contractAddress;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新交易状态为失败
    /// </summary>
    public void MarkAsFailed(string errorMessage)
    {
        Status = TransactionStatus.Failed;
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新交易状态为已取消
    /// </summary>
    public void MarkAsCancelled()
    {
        Status = TransactionStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 计算交易费用
    /// </summary>
    public decimal CalculateTransactionFee()
    {
        if (GasUsed.HasValue)
        {
            return GasUsed.Value * GasPrice;
        }
        
        return GasLimit * GasPrice;
    }

    /// <summary>
    /// 验证交易哈希格式
    /// </summary>
    public static bool IsValidTransactionHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return false;

        // 以太坊交易哈希格式验证：0x开头，66个字符
        return hash.StartsWith("0x") && hash.Length == 66;
    }
}

