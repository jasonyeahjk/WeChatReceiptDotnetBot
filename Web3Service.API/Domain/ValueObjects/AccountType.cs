namespace Web3Service.API.Domain.ValueObjects;

/// <summary>
/// 区块链账户类型
/// </summary>
public enum AccountType
{
    /// <summary>
    /// 外部拥有账户（EOA）
    /// </summary>
    ExternallyOwnedAccount = 1,
    
    /// <summary>
    /// 合约账户
    /// </summary>
    ContractAccount = 2,
    
    /// <summary>
    /// 多重签名账户
    /// </summary>
    MultiSignatureAccount = 3,
    
    /// <summary>
    /// 系统账户
    /// </summary>
    SystemAccount = 4
}

