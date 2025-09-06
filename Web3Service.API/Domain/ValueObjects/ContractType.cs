namespace Web3Service.API.Domain.ValueObjects;

/// <summary>
/// 智能合约类型
/// </summary>
public enum ContractType
{
    /// <summary>
    /// ERC20代币合约
    /// </summary>
    ERC20Token = 1,
    
    /// <summary>
    /// ERC721 NFT合约
    /// </summary>
    ERC721NFT = 2,
    
    /// <summary>
    /// ERC1155多代币合约
    /// </summary>
    ERC1155MultiToken = 3,
    
    /// <summary>
    /// 账本合约
    /// </summary>
    BillContract = 4,
    
    /// <summary>
    /// 支付合约
    /// </summary>
    PaymentContract = 5,
    
    /// <summary>
    /// 多重签名合约
    /// </summary>
    MultiSignature = 6,
    
    /// <summary>
    /// 代理合约
    /// </summary>
    Proxy = 7,
    
    /// <summary>
    /// 自定义合约
    /// </summary>
    Custom = 99,
    /// <summary>
    /// 未知类型
    /// </summary>
    Unknown = 0,
}