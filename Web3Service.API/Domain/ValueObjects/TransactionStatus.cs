namespace Web3Service.API.Domain.ValueObjects;

/// <summary>
/// 区块链交易状态
/// </summary>
public enum TransactionStatus
{
    /// <summary>
    /// 待处理
    /// </summary>
    Pending = 1,
    
    /// <summary>
    /// 已确认
    /// </summary>
    Confirmed = 2,
    
    /// <summary>
    /// 失败
    /// </summary>
    Failed = 3,
    
    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 4,
    
    /// <summary>
    /// 已替换
    /// </summary>
    Replaced = 5
}

