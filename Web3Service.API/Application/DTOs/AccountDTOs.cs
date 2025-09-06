using Web3Service.API.Domain.ValueObjects;

namespace Web3Service.API.Application.DTOs.AccountDTOs;

/// <summary>
/// 创建账户请求DTO
/// </summary>
public class CreateAccountRequestDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 账户类型
    /// </summary>
    public AccountType AccountType { get; set; } = AccountType.ExternallyOwnedAccount;
}

/// <summary>
/// 导入账户请求DTO
/// </summary>
public class ImportAccountRequestDto
{
    /// <summary>
    /// 私钥
    /// </summary>
    public string PrivateKey { get; set; } = string.Empty;
    
    /// <summary>
    /// 账户类型
    /// </summary>
    public AccountType AccountType { get; set; } = AccountType.ExternallyOwnedAccount;
}

/// <summary>
/// 账户响应DTO
/// </summary>
public class AccountResponseDto
{
    /// <summary>
    /// 账户ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 账户地址
    /// </summary>
    public string Address { get; set; } = string.Empty;
    
    /// <summary>
    /// 公钥
    /// </summary>
    public string? PublicKey { get; set; }
    
    /// <summary>
    /// 账户类型
    /// </summary>
    public AccountType AccountType { get; set; }
    
    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; }
    
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
/// 账户详情响应DTO
/// </summary>
public class AccountDetailResponseDto : AccountResponseDto
{
    /// <summary>
    /// 账户余额
    /// </summary>
    public decimal Balance { get; set; }
    
    /// <summary>
    /// 交易数量
    /// </summary>
    public int TransactionCount { get; set; }
    
    /// <summary>
    /// 部署的合约数量
    /// </summary>
    public int DeployedContractCount { get; set; }
}

/// <summary>
/// 账户列表查询请求DTO
/// </summary>
public class AccountListRequestDto
{
    /// <summary>
    /// 账户类型筛选
    /// </summary>
    public AccountType? AccountType { get; set; }
    
    /// <summary>
    /// 是否激活筛选
    /// </summary>
    public bool? IsActive { get; set; }
    
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
/// 账户余额响应DTO
/// </summary>
public class AccountBalanceResponseDto
{
    /// <summary>
    /// 账户地址
    /// </summary>
    public string Address { get; set; } = string.Empty;
    
    /// <summary>
    /// 余额（ETH）
    /// </summary>
    public decimal Balance { get; set; }
    
    /// <summary>
    /// 余额（Wei）
    /// </summary>
    public string BalanceWei { get; set; } = string.Empty;
    
    /// <summary>
    /// 查询时间
    /// </summary>
    public DateTime QueryTime { get; set; }
}

