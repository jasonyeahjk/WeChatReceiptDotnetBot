using Web3Service.API.Domain.ValueObjects;

namespace Web3Service.API.Application.DTOs.TransactionDTOs;

/// <summary>
/// 发送交易请求DTO
/// </summary>
public class SendTransactionRequestDto
{
    /// <summary>
    /// 发送方地址
    /// </summary>
    public string FromAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// 接收方地址
    /// </summary>
    public string ToAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// 转账金额（ETH）
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// 私钥（可选，用于签名）
    /// </summary>
    public string? PrivateKey { get; set; }
    
    /// <summary>
    /// Gas价格（可选）
    /// </summary>
    public decimal? GasPrice { get; set; }
    
    /// <summary>
    /// Gas限制（可选）
    /// </summary>
    public long? GasLimit { get; set; }
    
    /// <summary>
    /// 交易数据（可选）
    /// </summary>
    public string? Data { get; set; }
}

/// <summary>
/// 交易响应DTO
/// </summary>
public class TransactionResponseDto
{
    /// <summary>
    /// 交易ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 交易哈希
    /// </summary>
    public string TransactionHash { get; set; } = string.Empty;
    
    /// <summary>
    /// 发送方地址
    /// </summary>
    public string FromAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// 接收方地址
    /// </summary>
    public string ToAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// 转账金额
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Gas价格
    /// </summary>
    public decimal GasPrice { get; set; }
    
    /// <summary>
    /// Gas限制
    /// </summary>
    public long GasLimit { get; set; }
    
    /// <summary>
    /// 实际使用的Gas
    /// </summary>
    public long? GasUsed { get; set; }
    
    /// <summary>
    /// 交易状态
    /// </summary>
    public TransactionStatus Status { get; set; }
    
    /// <summary>
    /// 区块号
    /// </summary>
    public long? BlockNumber { get; set; }
    
    /// <summary>
    /// 区块哈希
    /// </summary>
    public string? BlockHash { get; set; }
    
    /// <summary>
    /// 交易在区块中的索引
    /// </summary>
    public int? TransactionIndex { get; set; }
    
    /// <summary>
    /// 合约地址（如果是合约创建交易）
    /// </summary>
    public string? ContractAddress { get; set; }
    
    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 交易列表查询请求DTO
/// </summary>
public class TransactionListRequestDto
{
    /// <summary>
    /// 账户ID筛选
    /// </summary>
    public Guid? AccountId { get; set; }
    
    /// <summary>
    /// 发送方地址筛选
    /// </summary>
    public string? FromAddress { get; set; }
    
    /// <summary>
    /// 接收方地址筛选
    /// </summary>
    public string? ToAddress { get; set; }
    
    /// <summary>
    /// 交易状态筛选
    /// </summary>
    public TransactionStatus? Status { get; set; }
    
    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime? FromDate { get; set; }
    
    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime? ToDate { get; set; }
    
    /// <summary>
    /// 跳过数量
    /// </summary>
    public int Skip { get; set; } = 0;
    
    /// <summary>
    /// 获取数量
    /// </summary>
    public int Take { get; set; } = 50;
}

/// <summary>
/// Gas估算请求DTO
/// </summary>
public class EstimateGasRequestDto
{
    /// <summary>
    /// 发送方地址
    /// </summary>
    public string FromAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// 接收方地址
    /// </summary>
    public string ToAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// 转账金额
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// 交易数据
    /// </summary>
    public string? Data { get; set; }
}

/// <summary>
/// Gas估算响应DTO
/// </summary>
public class EstimateGasResponseDto
{
    /// <summary>
    /// 估算的Gas数量
    /// </summary>
    public long EstimatedGas { get; set; }
    
    /// <summary>
    /// 当前Gas价格
    /// </summary>
    public decimal GasPrice { get; set; }
    
    /// <summary>
    /// 估算的交易费用
    /// </summary>
    public decimal EstimatedFee { get; set; }
    
    /// <summary>
    /// 估算时间
    /// </summary>
    public DateTime EstimatedAt { get; set; }
}

/// <summary>
/// 交易统计响应DTO
/// </summary>
public class TransactionStatisticsResponseDto
{
    /// <summary>
    /// 账户ID
    /// </summary>
    public Guid? AccountId { get; set; }

    /// <summary>
    /// 总交易数
    /// </summary>
    public int TotalTransactions { get; set; }
    
    /// <summary>
    /// 待处理交易数
    /// </summary>
    public int PendingTransactions { get; set; }
    
    /// <summary>
    /// 已确认交易数
    /// </summary>
    public int ConfirmedTransactions { get; set; }
    
    /// <summary>
    /// 失败交易数
    /// </summary>
    public int FailedTransactions { get; set; }
    
    /// <summary>
    /// 总交易金额
    /// </summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// 总Gas费用
    /// </summary>
    public decimal TotalGasFees { get; set; }
    
    /// <summary>
    /// 统计时间范围
    /// </summary>
    public DateTime? FromDate { get; set; }
    
    /// <summary>
    /// 统计时间范围
    /// </summary>
    public DateTime? ToDate { get; set; }
}

