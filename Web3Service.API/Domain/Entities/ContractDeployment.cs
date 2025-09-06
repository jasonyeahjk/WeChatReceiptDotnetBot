using Web3Service.API.Domain.ValueObjects;

namespace Web3Service.API.Domain.Entities;

/// <summary>
/// 智能合约部署记录实体
/// </summary>
public class ContractDeployment
{
    public Guid Id { get; private set; }
    public string ContractName { get; private set; }
    public string ContractAddress { get; private set; }
    public string DeploymentTransactionHash { get; private set; }
    public string ByteCode { get; private set; }
    public string? Abi { get; private set; }
    public string? SourceCode { get; private set; }
    public ContractType ContractType { get; private set; }
    public DeploymentStatus Status { get; private set; }
    public long? BlockNumber { get; private set; }
    public decimal DeploymentCost { get; private set; }
    public long GasUsed { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    // 外键
    public Guid DeployerAccountId { get; private set; }
    public BlockchainAccount DeployerAccount { get; private set; } = null!;

    private ContractDeployment() { } // EF Core

    public ContractDeployment(
        string contractName,
        string contractAddress,
        string deploymentTransactionHash,
        string byteCode,
        ContractType contractType,
        Guid deployerAccountId,
        string? abi = null,
        string? sourceCode = null)
    {
        Id = Guid.NewGuid();
        ContractName = contractName ?? throw new ArgumentNullException(nameof(contractName));
        ContractAddress = contractAddress ?? throw new ArgumentNullException(nameof(contractAddress));
        DeploymentTransactionHash = deploymentTransactionHash ?? throw new ArgumentNullException(nameof(deploymentTransactionHash));
        ByteCode = byteCode ?? throw new ArgumentNullException(nameof(byteCode));
        ContractType = contractType;
        DeployerAccountId = deployerAccountId;
        Abi = abi;
        SourceCode = sourceCode;
        Status = DeploymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 标记部署成功
    /// </summary>
    public void MarkAsDeployed(long blockNumber, decimal deploymentCost, long gasUsed)
    {
        Status = DeploymentStatus.Deployed;
        BlockNumber = blockNumber;
        DeploymentCost = deploymentCost;
        GasUsed = gasUsed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 标记部署失败
    /// </summary>
    public void MarkAsFailed(string errorMessage)
    {
        Status = DeploymentStatus.Failed;
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 更新合约信息
    /// </summary>
    public void UpdateContractInfo(string? abi = null, string? sourceCode = null)
    {
        if (!string.IsNullOrWhiteSpace(abi))
        {
            Abi = abi;
        }
        
        if (!string.IsNullOrWhiteSpace(sourceCode))
        {
            SourceCode = sourceCode;
        }
        
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 验证合约地址格式
    /// </summary>
    public static bool IsValidContractAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return false;

        // 以太坊合约地址格式验证：0x开头，42个字符
        return address.StartsWith("0x") && address.Length == 42;
    }

    /// <summary>
    /// 验证字节码格式
    /// </summary>
    public static bool IsValidByteCode(string byteCode)
    {
        if (string.IsNullOrWhiteSpace(byteCode))
            return false;

        // 字节码应该是十六进制字符串
        return byteCode.StartsWith("0x") && byteCode.Length > 2;
    }
}

