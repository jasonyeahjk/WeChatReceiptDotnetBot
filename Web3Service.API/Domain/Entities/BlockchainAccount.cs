using Web3Service.API.Domain.ValueObjects;

namespace Web3Service.API.Domain.Entities;

/// <summary>
/// 区块链账户聚合根
/// </summary>
public class BlockchainAccount
{
    public Guid Id { get; private set; }
    public string Address { get; private set; }
    public string? PrivateKey { get; private set; }
    public string? PublicKey { get; private set; }
    public AccountType AccountType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }
    
    // 导航属性
    public List<TransactionRecord> Transactions { get; private set; } = new();
    public List<ContractDeployment> DeployedContracts { get; private set; } = new();

    private BlockchainAccount() { } // EF Core

    public BlockchainAccount(
        string address,
        string? privateKey,
        string? publicKey,
        AccountType accountType)
    {
        Id = Guid.NewGuid();
        Address = address ?? throw new ArgumentNullException(nameof(address));
        PrivateKey = privateKey;
        PublicKey = publicKey;
        AccountType = accountType;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// 更新账户信息
    /// </summary>
    public void UpdateAccount(string? publicKey = null)
    {
        if (!string.IsNullOrWhiteSpace(publicKey))
        {
            PublicKey = publicKey;
        }
        
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 停用账户
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 激活账户
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 添加交易记录
    /// </summary>
    public void AddTransaction(TransactionRecord transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        Transactions.Add(transaction);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 添加合约部署记录
    /// </summary>
    public void AddContractDeployment(ContractDeployment deployment)
    {
        if (deployment == null)
            throw new ArgumentNullException(nameof(deployment));

        DeployedContracts.Add(deployment);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 验证地址格式
    /// </summary>
    public static bool IsValidAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return false;

        // 以太坊地址格式验证：0x开头，42个字符
        return address.StartsWith("0x") && address.Length == 42;
    }
}

