using Web3Service.API.Domain.ValueObjects;

namespace Web3Service.API.Application.DTOs.ContractDTOs;

/// <summary>
/// 部署合约请求DTO
/// </summary>
public class DeployContractRequestDto
{
    /// <summary>
    /// 合约名称
    /// </summary>
    public string ContractName { get; set; } = string.Empty;
    
    /// <summary>
    /// 合约字节码
    /// </summary>
    public string ByteCode { get; set; } = string.Empty;
    
    /// <summary>
    /// 合约ABI
    /// </summary>
    public string Abi { get; set; } = string.Empty;
    
    /// <summary>
    /// 合约类型
    /// </summary>
    public ContractType ContractType { get; set; }
    
    /// <summary>
    /// 部署者地址
    /// </summary>
    public string FromAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// 部署者私钥（可选）
    /// </summary>
    public string? PrivateKey { get; set; }
    
    /// <summary>
    /// 构造函数参数
    /// </summary>
    public object[]? ConstructorArguments { get; set; }
    
    /// <summary>
    /// Gas价格（可选）
    /// </summary>
    public decimal? GasPrice { get; set; }
    
    /// <summary>
    /// Gas限制（可选）
    /// </summary>
    public long? GasLimit { get; set; }
    
    /// <summary>
    /// 源代码（可选）
    /// </summary>
    public string? SourceCode { get; set; }
    
    /// <summary>
    /// 部署者用户ID
    /// </summary>
    public string? DeployedByUserId { get; set; }
}

/// <summary>
/// 合约部署响应DTO
/// </summary>
public class ContractDeploymentResponseDto
{
    /// <summary>
    /// 部署记录ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 合约名称
    /// </summary>
    public string ContractName { get; set; } = string.Empty;
    
    /// <summary>
    /// 合约地址
    /// </summary>
    public string ContractAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// 部署交易哈希
    /// </summary>
    public string DeploymentTransactionHash { get; set; } = string.Empty;
    
    /// <summary>
    /// 合约类型
    /// </summary>
    public ContractType ContractType { get; set; }
    
    /// <summary>
    /// 部署状态
    /// </summary>
    public DeploymentStatus Status { get; set; }
    
    /// <summary>
    /// 区块号
    /// </summary>
    public long? BlockNumber { get; set; }
    
    /// <summary>
    /// 部署成本
    /// </summary>
    public decimal DeploymentCost { get; set; }
    
    /// <summary>
    /// 使用的Gas
    /// </summary>
    public long GasUsed { get; set; }
    
    /// <summary>
    /// 部署者账户ID
    /// </summary>
    public Guid DeployerAccountId { get; set; }
    
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
/// 调用合约方法请求DTO
/// </summary>
public class CallContractMethodRequestDto
{
    /// <summary>
    /// 合约地址
    /// </summary>
    public string ContractAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// 合约ABI
    /// </summary>
    public string Abi { get; set; } = string.Empty;
    
    /// <summary>
    /// 方法名称
    /// </summary>
    public string MethodName { get; set; } = string.Empty;
    
    /// <summary>
    /// 方法参数
    /// </summary>
    public object[]? Parameters { get; set; }

    /// <summary>
    /// 发送方地址
    /// </summary>
    public string FromAddress { get; set; } = string.Empty;
}

/// <summary>
/// 发送合约交易请求DTO
/// </summary>
public class SendContractTransactionRequestDto : CallContractMethodRequestDto
{
    /// <summary>
    /// 发送方地址
    /// </summary>
    public string FromAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// 私钥（可选）
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
    /// 发送的ETH数量（可选）
    /// </summary>
    public decimal? Value { get; set; }
}

/// <summary>
/// 合约方法调用响应DTO
/// </summary>
public class ContractMethodCallResponseDto
{
    /// <summary>
    /// 合约地址
    /// </summary>
    public string ContractAddress { get; set; } = string.Empty;

    /// <summary>
    /// 方法名称
    /// </summary>
    public string MethodName { get; set; } = string.Empty;

    /// <summary>
    /// 调用结果
    /// </summary>
    public object? Result { get; set; }
    
    /// <summary>
    /// 交易哈希（仅适用于写入操作）
    /// </summary>
    public string? TransactionHash { get; set; }
    
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// 调用时间
    /// </summary>
    public DateTime CalledAt { get; set; }
}

/// <summary>
/// 合约事件查询请求DTO
/// </summary>
public class ContractEventsRequestDto
{
    /// <summary>
    /// 合约地址
    /// </summary>
    public string ContractAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// 合约ABI
    /// </summary>
    public string Abi { get; set; } = string.Empty;
    
    /// <summary>
    /// 事件名称
    /// </summary>
    public string EventName { get; set; } = string.Empty;
    
    /// <summary>
    /// 起始区块号
    /// </summary>
    public long? FromBlock { get; set; }
    
    /// <summary>
    /// 结束区块号
    /// </summary>
    public long? ToBlock { get; set; }
}

/// <summary>
/// 合约列表查询请求DTO
/// </summary>
public class ContractListRequestDto
{
    /// <summary>
    /// 部署者账户ID筛选
    /// </summary>
    public Guid? DeployerAccountId { get; set; }
    
    /// <summary>
    /// 合约类型筛选
    /// </summary>
    public ContractType? ContractType { get; set; }
    
    /// <summary>
    /// 部署状态筛选
    /// </summary>
    public DeploymentStatus? Status { get; set; }
    
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
/// 合约验证请求DTO
/// </summary>
public class VerifyContractRequestDto
{
    /// <summary>
    /// 合约名称
    /// </summary>
    public string ContractName { get; set; } = string.Empty;

    /// <summary>
    /// 合约地址
    /// </summary>
    public string ContractAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// 源代码
    /// </summary>
    public string SourceCode { get; set; } = string.Empty;
    
    /// <summary>
    /// 编译器版本
    /// </summary>
    public string CompilerVersion { get; set; } = string.Empty;

    /// <summary>
    /// 是否使用优化
    /// </summary>
    public bool OptimizationUsed { get; set; }

    /// <summary>
    /// 优化运行次数
    /// </summary>
    public int Runs { get; set; }

    /// <summary>
    /// 构造函数参数
    /// </summary>
    public string? ConstructorArguments { get; set; }
}



